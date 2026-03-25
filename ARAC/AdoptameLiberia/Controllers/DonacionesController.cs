using AdoptameLiberia.Models.Donaciones;
using AdoptameLiberia.Models.Donaciones.VM;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AdoptameLiberia.Controllers
{
    [Authorize]
    public class DonacionesController : Controller
    {
        private readonly ARACDbContext db = new ARACDbContext();

        private string NormalizarNombreTipo(string nombre)
        {
            return (nombre ?? string.Empty).Trim().ToLowerInvariant();
        }

        private List<SelectListItem> ObtenerTiposDonacionUnicos(int? seleccionado = null)
        {
            var tipos = db.TiposDonacion
                .AsEnumerable()
                .GroupBy(t => NormalizarNombreTipo(t.Nombre))
                .Select(g => g.OrderBy(x => x.IdTipoDonacion).First())
                .OrderBy(t => t.Nombre)
                .Select(t => new SelectListItem
                {
                    Value = t.IdTipoDonacion.ToString(),
                    Text = t.Nombre,
                    Selected = seleccionado.HasValue && t.IdTipoDonacion == seleccionado.Value
                })
                .ToList();

            return tipos;
        }

        private List<SelectListItem> ObtenerItemsInventario()
        {
            return db.ItemsInventario
                .Where(i => i.Estado)
                .OrderBy(i => i.Nombre)
                .Select(i => new SelectListItem
                {
                    Value = i.IdItemInventario.ToString(),
                    Text = i.Nombre + " (" + i.StockActual + ")"
                })
                .ToList();
        }

        private List<SelectListItem> ObtenerMetodosPago()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Sinpe", Text = "Sinpe" },
                new SelectListItem { Value = "Transferencia", Text = "Transferencia" },
                new SelectListItem { Value = "Efectivo", Text = "Efectivo" },
                new SelectListItem { Value = "Entrega directa", Text = "Entrega directa" }
            };
        }

        private int? ObtenerIdTipoMonetaria()
        {
            var tipo = db.TiposDonacion
                .AsEnumerable()
                .Where(t => NormalizarNombreTipo(t.Nombre) == "monetaria")
                .OrderBy(t => t.IdTipoDonacion)
                .FirstOrDefault();

            return tipo?.IdTipoDonacion;
        }

        private void CargarCombos(DonacionCreateVM vm)
        {
            vm.TiposDonacion = ObtenerTiposDonacionUnicos(vm.IdTipoDonacion == 0 ? (int?)null : vm.IdTipoDonacion);
            vm.ItemsInventario = ObtenerItemsInventario();
            vm.MetodosPago = ObtenerMetodosPago();

            if (vm.Insumos == null || !vm.Insumos.Any())
            {
                vm.Insumos = new List<DetalleInsumoVM> { new DetalleInsumoVM() };
            }
        }

        public ActionResult Index(int? tipoDonacionId)
        {
            var donaciones = db.Donaciones
                .Include(d => d.TipoDonacion)
                .AsQueryable();

            if (tipoDonacionId.HasValue)
            {
                var tipoSeleccionado = db.TiposDonacion.FirstOrDefault(t => t.IdTipoDonacion == tipoDonacionId.Value);

                if (tipoSeleccionado != null)
                {
                    var nombreNormalizado = NormalizarNombreTipo(tipoSeleccionado.Nombre);
                    donaciones = donaciones.Where(d => d.TipoDonacion.Nombre.ToLower() == nombreNormalizado);
                }
            }

            ViewBag.TiposDonacion = new SelectList(
                ObtenerTiposDonacionUnicos(),
                "Value",
                "Text",
                tipoDonacionId
            );

            return View(donaciones.OrderByDescending(d => d.FechaRegistro).ToList());
        }

        public ActionResult Create(decimal? monto)
        {
            var vm = new DonacionCreateVM();

            if (monto.HasValue)
            {
                vm.Monto = monto.Value;
                var idMonetaria = ObtenerIdTipoMonetaria();
                if (idMonetaria.HasValue)
                {
                    vm.IdTipoDonacion = idMonetaria.Value;
                }
            }

            CargarCombos(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DonacionCreateVM vm)
        {
            CargarCombos(vm);

            var tipo = db.TiposDonacion.FirstOrDefault(t => t.IdTipoDonacion == vm.IdTipoDonacion);

            if (tipo == null)
            {
                ModelState.AddModelError("", "Tipo de donación inválido.");
                return View(vm);
            }

            bool esMonetaria = NormalizarNombreTipo(tipo.Nombre) == "monetaria";

            if (esMonetaria)
            {
                if (!vm.Monto.HasValue || vm.Monto.Value <= 0)
                {
                    ModelState.AddModelError("Monto", "Debes ingresar un monto válido.");
                    return View(vm);
                }

                if (string.IsNullOrWhiteSpace(vm.Metodo))
                {
                    ModelState.AddModelError("Metodo", "Debes seleccionar un método de pago.");
                    return View(vm);
                }
            }
            else
            {
                vm.Metodo = "Entrega directa";
                vm.Monto = 0;

                var hayDetalle = vm.Insumos != null && vm.Insumos.Any(x =>
                    x != null &&
                    (
                        x.IdItemInventario.HasValue ||
                        !string.IsNullOrWhiteSpace(x.Descripcion) ||
                        (x.Cantidad.HasValue && x.Cantidad.Value > 0)
                    )
                );

                if (!hayDetalle)
                {
                    ModelState.AddModelError("", "Debes agregar al menos un detalle del objeto donado.");
                    return View(vm);
                }
            }

            var userId = User.Identity.GetUserId();
            var usuario = db.Usuarios.FirstOrDefault(u => u.IdAspNetUser == userId);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Tu usuario no está vinculado con la tabla Usuario.");
                return View(vm);
            }

            var donacion = new Donacion
            {
                IdUsuario = usuario.ID_Usuario,
                IdTipoDonacion = tipo.IdTipoDonacion,
                Monto = esMonetaria ? vm.Monto.Value : 0,
                Fecha = DateTime.Now,
                Metodo = vm.Metodo,
                Descripcion = vm.Descripcion,
                FechaRegistro = DateTime.Now
            };

            db.Donaciones.Add(donacion);
            db.SaveChanges();

            if (!esMonetaria && vm.Insumos != null)
            {
                foreach (var ins in vm.Insumos)
                {
                    if (ins == null)
                    {
                        continue;
                    }

                    var cantidad = ins.Cantidad.HasValue && ins.Cantidad.Value > 0 ? ins.Cantidad.Value : 1;
                    var detalleVacio = !ins.IdItemInventario.HasValue && string.IsNullOrWhiteSpace(ins.Descripcion);

                    if (detalleVacio)
                    {
                        continue;
                    }

                    db.DetallesDonacion.Add(new DetalleDonacion
                    {
                        IdDonacion = donacion.IdDonacion,
                        IdItemInventario = ins.IdItemInventario,
                        Cantidad = cantidad,
                        Descripcion = ins.Descripcion
                    });

                    if (ins.IdItemInventario.HasValue)
                    {
                        var item = db.ItemsInventario.Find(ins.IdItemInventario.Value);
                        if (item != null)
                        {
                            item.StockActual += cantidad;
                        }
                    }
                }

                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = donacion.IdDonacion });
        }

        public ActionResult Details(int id)
        {
            var donacion = db.Donaciones
                .Include(d => d.TipoDonacion)
                .Include(d => d.Detalles)
                .Include(d => d.Observaciones)
                .FirstOrDefault(d => d.IdDonacion == id);

            if (donacion == null)
            {
                return HttpNotFound();
            }

            return View(donacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarObservacion(int idDonacion, string comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario))
            {
                return RedirectToAction("Details", new { id = idDonacion });
            }

            db.ObservacionesDonacion.Add(new ObservacionDonacion
            {
                IdDonacion = idDonacion,
                Comentario = comentario,
                Fecha = DateTime.Now
            });

            db.SaveChanges();

            return RedirectToAction("Details", new { id = idDonacion });
        }

        public ActionResult Comprobante(int id)
        {
            var donacion = db.Donaciones
                .Include(d => d.TipoDonacion)
                .Include(d => d.Detalles)
                .FirstOrDefault(d => d.IdDonacion == id);

            if (donacion == null)
            {
                return HttpNotFound();
            }

            return View(donacion);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}