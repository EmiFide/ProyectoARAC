using AdoptameLiberia.Filters;
using AdoptameLiberia.Models;
using AdoptameLiberia.Models.Donaciones;
using AdoptameLiberia.Models.Inventario;
using Microsoft.AspNet.Identity;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace AdoptameLiberia.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class InventarioController : Controller
    {
        private readonly ARACDbContext db = new ARACDbContext();

        private readonly ApplicationDbContext identityDb = new ApplicationDbContext();

        [PermissionAuthorize(Module = "Inventario", RequireWrite = false)]

        public async Task<ActionResult> Index()
        {
            var items = await db.ItemsInventario
                .OrderBy(i => i.Nombre)
                .ToListAsync();

            // Alertas: stock <= mínimo
            ViewBag.StockBajoCount = items.Count(i => i.StockActual <= i.StockMinimo);

            // Alertas: vencimientos próximos (30 días)
            var limite = DateTime.Today.AddDays(30);
            ViewBag.VencenProntoCount = items.Count(i => i.FechaCaducidad.HasValue && i.FechaCaducidad.Value <= limite);

            return View(items);
        }

        [PermissionAuthorize(Module = "Inventario", RequireWrite = true)]
        public ActionResult Create()
        {
            return View(new ItemInventarioCreateEdit());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize(Module = "Inventario", RequireWrite = true)]

        public async Task<ActionResult> Create(ItemInventarioCreateEdit model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existe = await db.ItemsInventario.AnyAsync(x => x.Nombre == model.Nombre);
            if (existe)
            {
                ModelState.AddModelError("Nombre", "Ya existe un artículo con ese nombre.");
                return View(model);
            }

            var entity = new ItemInventario
            {
                Nombre = model.Nombre.Trim(),
                Descripcion = model.Descripcion,
                Categoria = model.Categoria,
                UnidadMedida = model.UnidadMedida,
                StockActual = model.StockActual,
                StockMinimo = model.StockMinimo,
                FechaCaducidad = model.FechaCaducidad,
                Estado = true,
                FechaRegistro = DateTime.Now
            };

            db.ItemsInventario.Add(entity);
            await db.SaveChangesAsync();

            TempData["Success"] = "Artículo registrado correctamente.";
            return RedirectToAction("Index");
        }

        [PermissionAuthorize(Module = "Inventario", RequireWrite = true)]
        public async Task<ActionResult> Edit(int id)
        {
            var item = await db.ItemsInventario.FirstOrDefaultAsync(x => x.IdItemInventario == id);
            if (item == null) return HttpNotFound();

            var vm = new ItemInventarioCreateEdit
            {
                IdItemInventario = item.IdItemInventario,
                Nombre = item.Nombre,
                Descripcion = item.Descripcion,
                Categoria = item.Categoria,
                UnidadMedida = item.UnidadMedida,
                StockActual = item.StockActual,
                StockMinimo = item.StockMinimo,
                FechaCaducidad = item.FechaCaducidad
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize(Module = "Inventario", RequireWrite = true)]
        public async Task<ActionResult> Edit(ItemInventarioCreateEdit model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var item = await db.ItemsInventario.FirstOrDefaultAsync(x => x.IdItemInventario == model.IdItemInventario);
            if (item == null) return HttpNotFound();

            var duplicado = await db.ItemsInventario.AnyAsync(x => x.Nombre == model.Nombre && x.IdItemInventario != model.IdItemInventario);
            if (duplicado)
            {
                ModelState.AddModelError("Nombre", "Ya existe otro artículo con ese nombre.");
                return View(model);
            }

            item.Nombre = model.Nombre.Trim();
            item.Descripcion = model.Descripcion;
            item.Categoria = model.Categoria;
            item.UnidadMedida = model.UnidadMedida;
            item.StockMinimo = model.StockMinimo;
            item.FechaCaducidad = model.FechaCaducidad;

            await db.SaveChangesAsync();

            TempData["Success"] = "Artículo actualizado correctamente.";
            return RedirectToAction("Index");
        }

        [PermissionAuthorize(Module = "Inventario", RequireWrite = true)]
        public async Task<ActionResult> Entrada(int id)
        {
            var item = await db.ItemsInventario.FirstOrDefaultAsync(x => x.IdItemInventario == id);
            if (item == null) return HttpNotFound();

            var vm = new MovimientoEntrada
            {
                IdItemInventario = item.IdItemInventario,
                NombreItem = item.Nombre
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize(Module = "Inventario", RequireWrite = true)]
        public async Task<ActionResult> Entrada(MovimientoEntrada model)
        {
            var item = await db.ItemsInventario.FirstOrDefaultAsync(x => x.IdItemInventario == model.IdItemInventario);
            if (item == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                model.NombreItem = item.Nombre;
                return View(model);
            }

            var anterior = item.StockActual;
            item.StockActual += model.Cantidad;

            db.MovimientosInventario.Add(new MovimientoInventario
            {
                IdItemInventario = item.IdItemInventario,
                TipoMovimiento = "Entrada",
                Cantidad = model.Cantidad,
                StockAnterior = anterior,
                StockNuevo = item.StockActual,
                Motivo = model.Motivo,
                FechaMovimiento = DateTime.Now,
                UsuarioId = User.Identity.GetUserId()
            });

            await db.SaveChangesAsync();

            TempData["Success"] = "Entrada registrada correctamente.";
            return RedirectToAction("Index");
        }

        [PermissionAuthorize(Module = "Inventario", RequireWrite = true)]
        public async Task<ActionResult> Salida(int id)
        {
            var item = await db.ItemsInventario.FirstOrDefaultAsync(x => x.IdItemInventario == id);
            if (item == null) return HttpNotFound();

            var vm = new MovimientoSalida
            {
                IdItemInventario = item.IdItemInventario,
                NombreItem = item.Nombre
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize(Module = "Inventario", RequireWrite = true)]
        public async Task<ActionResult> Salida(MovimientoSalida model)
        {
            var item = await db.ItemsInventario.FirstOrDefaultAsync(x => x.IdItemInventario == model.IdItemInventario);
            if (item == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                model.NombreItem = item.Nombre;
                return View(model);
            }

            if (item.StockActual < model.Cantidad)
            {
                ModelState.AddModelError("Cantidad", "No hay stock suficiente para registrar esta salida.");
                model.NombreItem = item.Nombre;
                return View(model);
            }

            var anterior = item.StockActual;
            item.StockActual -= model.Cantidad;

            db.MovimientosInventario.Add(new MovimientoInventario
            {
                IdItemInventario = item.IdItemInventario,
                TipoMovimiento = "Salida",
                Cantidad = model.Cantidad,
                StockAnterior = anterior,
                StockNuevo = item.StockActual,
                Destinatario = model.Destinatario,
                Motivo = model.Motivo,
                FechaMovimiento = DateTime.Now,
                UsuarioId = User.Identity.GetUserId()
            });

            await db.SaveChangesAsync();

            TempData["Success"] = "Salida registrada correctamente.";
            return RedirectToAction("Index");
        }

        [PermissionAuthorize(Module = "Inventario", RequireWrite = true)]
        public async Task<ActionResult> Ajuste(int id)
        {
            var item = await db.ItemsInventario.FirstOrDefaultAsync(x => x.IdItemInventario == id);
            if (item == null) return HttpNotFound();

            var vm = new AjusteInventario
            {
                IdItemInventario = item.IdItemInventario,
                NombreItem = item.Nombre,
                StockActual = item.StockActual
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [PermissionAuthorize(Module = "Inventario", RequireWrite = true)]
        public async Task<ActionResult> Ajuste(AjusteInventario model)
        {
            var item = await db.ItemsInventario.FirstOrDefaultAsync(x => x.IdItemInventario == model.IdItemInventario);
            if (item == null) return HttpNotFound();

            if (!ModelState.IsValid)
            {
                model.NombreItem = item.Nombre;
                model.StockActual = item.StockActual;
                return View(model);
            }

            var anterior = item.StockActual;
            item.StockActual = model.NuevoStock;

            db.MovimientosInventario.Add(new MovimientoInventario
            {
                IdItemInventario = item.IdItemInventario,
                TipoMovimiento = "Ajuste",
                Cantidad = Math.Abs(model.NuevoStock - anterior),
                StockAnterior = anterior,
                StockNuevo = model.NuevoStock,
                Motivo = model.Motivo,
                FechaMovimiento = DateTime.Now,
                UsuarioId = User.Identity.GetUserId()
            });

            await db.SaveChangesAsync();

            TempData["Success"] = "Ajuste aplicado correctamente.";
            return RedirectToAction("Index");
        }

        [PermissionAuthorize(Module = "Inventario", RequireWrite = false)]
        public async Task<ActionResult> AlertasStockMinimo()
        {
            var items = await db.ItemsInventario
                .Where(i => i.StockActual <= i.StockMinimo)
                .OrderBy(i => i.StockActual)
                .ToListAsync();

            return View(items);
        }

        [PermissionAuthorize(Module = "Inventario", RequireWrite = false)]
        public async Task<ActionResult> Vencimientos(int dias = 30)
        {
            var limite = DateTime.Today.AddDays(dias);

            var items = await db.ItemsInventario
                .Where(i => i.FechaCaducidad.HasValue && i.FechaCaducidad.Value <= limite)
                .OrderBy(i => i.FechaCaducidad)
                .ToListAsync();

            ViewBag.Dias = dias;
            return View(items);
        }

        [PermissionAuthorize(Module = "Inventario", RequireWrite = false)]
        public async Task<ActionResult> Historial(HistorialInventarioFilter filter)
        {
            var query = db.MovimientosInventario.AsQueryable();

            if (filter.Desde.HasValue)
                query = query.Where(m => DbFunctions.TruncateTime(m.FechaMovimiento) >= DbFunctions.TruncateTime(filter.Desde.Value));

            if (filter.Hasta.HasValue)
                query = query.Where(m => DbFunctions.TruncateTime(m.FechaMovimiento) <= DbFunctions.TruncateTime(filter.Hasta.Value));

            if (!string.IsNullOrWhiteSpace(filter.UsuarioEmail))
            {
                var ids = identityDb.Users
                    .Where(u => u.Email.Contains(filter.UsuarioEmail))
                    .Select(u => u.Id);

                query = query.Where(m => ids.Contains(m.UsuarioId));
            }

            var movimientos = await query
                .OrderByDescending(m => m.FechaMovimiento)
                .ToListAsync();

            var itemIds = movimientos.Select(m => m.IdItemInventario).Distinct().ToList();
            var items = await db.ItemsInventario.Where(i => itemIds.Contains(i.IdItemInventario)).ToListAsync();
            var itemMap = items.ToDictionary(i => i.IdItemInventario, i => i.Nombre);

            var userIds = movimientos.Where(m => m.UsuarioId != null).Select(m => m.UsuarioId).Distinct().ToList();
            var userMap = identityDb.Users.Where(u => userIds.Contains(u.Id)).ToDictionary(u => u.Id, u => u.Email);

            var vm = new HistorialInventario
            {
                Filter = filter ?? new HistorialInventarioFilter(),
                Movimientos = movimientos.Select(m => new HistorialInventarioRowVM
                {
                    Fecha = m.FechaMovimiento,
                    Item = itemMap.ContainsKey(m.IdItemInventario) ? itemMap[m.IdItemInventario] : ("#" + m.IdItemInventario),
                    Tipo = m.TipoMovimiento,
                    Cantidad = m.Cantidad,
                    StockAnterior = m.StockAnterior,
                    StockNuevo = m.StockNuevo,
                    Destinatario = m.Destinatario,
                    Motivo = m.Motivo,
                    UsuarioEmail = (m.UsuarioId != null && userMap.ContainsKey(m.UsuarioId)) ? userMap[m.UsuarioId] : ""
                }).ToList()
            };

            return View(vm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                identityDb.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
