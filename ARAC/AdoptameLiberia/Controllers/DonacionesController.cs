using AdoptameLiberia.Models.Donaciones;
using AdoptameLiberia.Models.Donaciones.VM;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace AdoptameLiberia.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DonacionesController : Controller
    {
        private readonly ARACDbContext db = new ARACDbContext();

        // =========================
        // INDEX
        // =========================
        public ActionResult Index(int? tipoDonacionId)
        {
            var donaciones = db.Donaciones
                .Include(d => d.TipoDonacion)
                .AsQueryable();

            if (tipoDonacionId.HasValue)
                donaciones = donaciones.Where(d => d.IdTipoDonacion == tipoDonacionId.Value);

            ViewBag.TiposDonacion = new SelectList(
                db.TiposDonacion,
                "IdTipoDonacion",
                "Nombre",
                tipoDonacionId
            );

            return View(donaciones.ToList());
        }

        // =========================
        // GET CREATE
        // =========================
        public ActionResult Create(decimal? monto)
        {
            var vm = new DonacionCreateVM
            {
                TiposDonacion = db.TiposDonacion.Select(t => new SelectListItem
                {
                    Value = t.IdTipoDonacion.ToString(),
                    Text = t.Nombre
                }).ToList(),

                ItemsInventario = db.ItemsInventario
                    .Where(i => i.Estado)
                    .Select(i => new SelectListItem
                    {
                        Value = i.IdItemInventario.ToString(),
                        Text = i.Nombre + " (" + i.StockActual + ")"
                    }).ToList(),

                MetodosPago = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Sinpe", Text = "Sinpe" },
                    new SelectListItem { Value = "Transferencia", Text = "Transferencia" },
                    new SelectListItem { Value = "Efectivo", Text = "Efectivo" },
                    new SelectListItem { Value = "Entrega directa", Text = "Entrega directa" }
                }
            };

            if (monto.HasValue)
            {
                vm.Monto = monto.Value;

                var tipoMonetaria = db.TiposDonacion
                    .FirstOrDefault(t => t.Nombre == "Monetaria");

                if (tipoMonetaria != null)
                    vm.IdTipoDonacion = tipoMonetaria.IdTipoDonacion;
            }

            vm.Insumos.Add(new DetalleInsumoVM());

            return View(vm);
        }

        // =========================
        // POST CREATE
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DonacionCreateVM vm)
        {
            // Recargar combos
            vm.TiposDonacion = db.TiposDonacion.Select(t => new SelectListItem
            {
                Value = t.IdTipoDonacion.ToString(),
                Text = t.Nombre
            }).ToList();

            vm.ItemsInventario = db.ItemsInventario
                .Where(i => i.Estado)
                .Select(i => new SelectListItem
                {
                    Value = i.IdItemInventario.ToString(),
                    Text = i.Nombre + " (" + i.StockActual + ")"
                }).ToList();

            vm.MetodosPago = new List<SelectListItem>
            {
                new SelectListItem { Value = "Sinpe", Text = "Sinpe" },
                new SelectListItem { Value = "Transferencia", Text = "Transferencia" },
                new SelectListItem { Value = "Efectivo", Text = "Efectivo" },
                new SelectListItem { Value = "Entrega directa", Text = "Entrega directa" }
            };

            if (!ModelState.IsValid)
                return View(vm);

            var tipo = db.TiposDonacion.FirstOrDefault(t => t.IdTipoDonacion == vm.IdTipoDonacion);

            if (tipo == null)
            {
                ModelState.AddModelError("", "Tipo inválido.");
                return View(vm);
            }

            bool esMonetaria = tipo.Nombre.Equals("Monetaria", StringComparison.OrdinalIgnoreCase);

            // VALIDACIONES
            if (esMonetaria)
            {
                if (vm.Monto == null || vm.Monto <= 0)
                {
                    ModelState.AddModelError("Monto", "Monto inválido.");
                    return View(vm);
                }
            }
            else
            {
                vm.Metodo = "Entrega directa";

                var hayInsumo = vm.Insumos != null && vm.Insumos.Any(x =>
                    x.IdItemInventario.HasValue || !string.IsNullOrWhiteSpace(x.Descripcion));

                if (!hayInsumo)
                {
                    ModelState.AddModelError("", "Debe ingresar al menos un insumo.");
                    return View(vm);
                }
            }

            // USUARIO
            var userId = User.Identity.GetUserId();

            var usuario = db.Usuarios.FirstOrDefault(u => u.IdAspNetUser == userId);

            if (usuario == null)
            {
                ModelState.AddModelError("", "Usuario no vinculado.");
                return View(vm);
            }

            // CREAR DONACIÓN
            var donacion = new Donacion
            {
                IdUsuario = usuario.ID_Usuario,
                IdTipoDonacion = vm.IdTipoDonacion,
                Monto = esMonetaria ? vm.Monto.Value : 0,
                Fecha = DateTime.Now,
                Metodo = vm.Metodo,
                Descripcion = vm.Descripcion,
                FechaRegistro = DateTime.Now
            };

            db.Donaciones.Add(donacion);
            db.SaveChanges();

            // INSUMOS
            if (!esMonetaria && vm.Insumos != null)
            {
                foreach (var ins in vm.Insumos)
                {
                    if (ins == null) continue;

                    if (!ins.IdItemInventario.HasValue && string.IsNullOrWhiteSpace(ins.Descripcion))
                        continue;

                    db.DetallesDonacion.Add(new DetalleDonacion
                    {
                        IdDonacion = donacion.IdDonacion,
                        IdItemInventario = ins.IdItemInventario,
                        Cantidad = ins.Cantidad ?? 1,
                        Descripcion = ins.Descripcion
                    });

                    if (ins.IdItemInventario.HasValue)
                    {
                        var item = db.ItemsInventario.Find(ins.IdItemInventario.Value);

                        if (item != null)
                            item.StockActual += ins.Cantidad ?? 1;
                    }
                }

                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = donacion.IdDonacion });
        }

        // =========================
        // DETAILS
        // =========================
        public ActionResult Details(int id)
        {
            var donacion = db.Donaciones
                .Include(d => d.TipoDonacion)
                .Include(d => d.Detalles)
                .Include(d => d.Observaciones)
                .FirstOrDefault(d => d.IdDonacion == id);

            if (donacion == null)
                return HttpNotFound();

            return View(donacion);
        }

        // =========================
        // OBSERVACIONES
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarObservacion(int idDonacion, string comentario)
        {
            if (string.IsNullOrWhiteSpace(comentario))
                return RedirectToAction("Details", new { id = idDonacion });

            db.ObservacionesDonacion.Add(new ObservacionDonacion
            {
                IdDonacion = idDonacion,
                Comentario = comentario,
                Fecha = DateTime.Now
            });

            db.SaveChanges();

            return RedirectToAction("Details", new { id = idDonacion });
        }

        // =========================
        // COMPROBANTE
        // =========================
        public ActionResult Comprobante(int id)
        {
            var donacion = db.Donaciones
                .Include(d => d.TipoDonacion)
                .Include(d => d.Detalles)
                .FirstOrDefault(d => d.IdDonacion == id);

            if (donacion == null)
                return HttpNotFound();

            return View(donacion);
        }
    }
}