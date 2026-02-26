using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AdoptameLiberia.Models.Donaciones;
using AdoptameLiberia.Models.Donaciones.VM;

namespace AdoptameLiberia.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class DonacionesController : Controller
    {
        private readonly ARACDbContext db = new ARACDbContext();

        // H31: Historial
        public ActionResult Index()
        {
            var list = db.Donaciones
                         .Include(d => d.TipoDonacion)
                         .OrderByDescending(d => d.FechaRegistro)
                         .ToList();
            return View(list);
        }

        // H29/H30: Crear
        public ActionResult Create()
        {
            var vm = new DonacionCreateVM
            {
                TiposDonacion = db.TiposDonacion.Select(t => new SelectListItem
                {
                    Value = t.IdTipoDonacion.ToString(),
                    Text = t.Nombre
                }).ToList(),
                ItemsInventario = db.ItemsInventario.Where(i => i.Estado).Select(i => new SelectListItem
                {
                    Value = i.IdItemInventario.ToString(),
                    Text = i.Nombre + " (" + i.StockActual + ")"
                }).ToList()
            };

            // mínimo 1 fila para insumos
            vm.Insumos.Add(new DetalleInsumoVM());
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DonacionCreateVM vm)
        {
            // recargar combos por si hay error
            vm.TiposDonacion = db.TiposDonacion.Select(t => new SelectListItem
            {
                Value = t.IdTipoDonacion.ToString(),
                Text = t.Nombre
            }).ToList();

            vm.ItemsInventario = db.ItemsInventario.Where(i => i.Estado).Select(i => new SelectListItem
            {
                Value = i.IdItemInventario.ToString(),
                Text = i.Nombre + " (" + i.StockActual + ")"
            }).ToList();

            if (!ModelState.IsValid)
                return View(vm);

            // Validación por tipo
            var tipo = db.TiposDonacion.FirstOrDefault(t => t.IdTipoDonacion == vm.IdTipoDonacion);
            if (tipo == null)
            {
                ModelState.AddModelError("", "Tipo de donación inválido.");
                return View(vm);
            }

            bool esMonetaria = string.Equals(tipo.Nombre, "Monetaria", StringComparison.OrdinalIgnoreCase);

            if (esMonetaria)
            {
                if (vm.Monto == null || vm.Monto <= 0)
                {
                    ModelState.AddModelError("Monto", "El monto debe ser mayor a 0.");
                    return View(vm);
                }
            }
            else
            {
                // Insumos: debe existir al menos un detalle con descripción o item
                var hayDetalle = vm.Insumos != null && vm.Insumos.Any(x =>
                    (x.IdItemInventario.HasValue && x.IdItemInventario.Value > 0) ||
                    (!string.IsNullOrWhiteSpace(x.Descripcion)));

                if (!hayDetalle)
                {
                    ModelState.AddModelError("", "Debe ingresar al menos un insumo/detalle.");
                    return View(vm);
                }
            }

            // Crear Donación
            var donacion = new Donacion
            {
                IdUsuario = vm.IdUsuario,
                IdTipoDonacion = vm.IdTipoDonacion,
                Monto = esMonetaria ? vm.Monto.Value : 0m, // en tu tabla Monto NO es nullable
                Fecha = DateTime.Now,
                Metodo = vm.Metodo,
                Descripcion = vm.Descripcion,
                FechaRegistro = DateTime.Now
            };

            db.Donaciones.Add(donacion);
            db.SaveChanges();

            // Si es insumos: detalle + movimiento inventario (Entrada)
            if (!esMonetaria && vm.Insumos != null)
            {
                foreach (var ins in vm.Insumos)
                {
                    if (ins == null) continue;
                    if (ins.IdItemInventario == null && string.IsNullOrWhiteSpace(ins.Descripcion)) continue;

                    var det = new DetalleDonacion
                    {
                        IdDonacion = donacion.IdDonacion,
                        IdItemInventario = ins.IdItemInventario,
                        Descripcion = ins.Descripcion
                    };
                    db.DetallesDonacion.Add(det);

                    if (ins.IdItemInventario.HasValue)
                    {
                        db.MovimientosInventario.Add(new MovimientoInventario
                        {
                            IdItemInventario = ins.IdItemInventario.Value,
                            TipoMovimiento = "Entrada",
                            FechaMovimiento = DateTime.Now,
                            Motivo = "Donación recibida (ID " + donacion.IdDonacion + ")"
                        });

                        // (Opcional) aumentar stock si quieres manejar cantidad (tu tabla no tiene Cantidad)
                        // Aquí NO toco Stock_Actual porque tu modelo no tiene cantidad numérica.
                    }
                }

                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = donacion.IdDonacion });
        }

        // Details: muestra donación + detalles + observaciones
        public ActionResult Details(int id)
        {
            var donacion = db.Donaciones
                             .Include(d => d.TipoDonacion)
                             .Include(d => d.Detalles)
                             .Include(d => d.Observaciones)
                             .FirstOrDefault(d => d.IdDonacion == id);

            if (donacion == null) return HttpNotFound();
            return View(donacion);
        }

        // H35: Agregar observación
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

        // H32: Comprobante (vista imprimible)
        public ActionResult Comprobante(int id)
        {
            var donacion = db.Donaciones
                             .Include(d => d.TipoDonacion)
                             .Include(d => d.Detalles)
                             .FirstOrDefault(d => d.IdDonacion == id);

            if (donacion == null) return HttpNotFound();
            return View(donacion);
        }
    }
}