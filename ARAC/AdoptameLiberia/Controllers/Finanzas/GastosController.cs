using System;
using System.Linq;
using System.Web.Mvc;
using AdoptameLiberia.Models;
using AdoptameLiberia.Models.Finanzas;

namespace AdoptameLiberia.Controllers.Finanzas
{
    [Authorize]
    public class GastoController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Gasto
        public ActionResult Index()
        {
            var lista = db.Gastos
                          .OrderByDescending(g => g.ID_Gasto)
                          .ToList();

            return View("~/Views/Gasto/Index.cshtml", lista);
        }

        // GET: Gasto/Create
        public ActionResult Create()
        {
            CargarCategorias(null);

            var model = new Gasto
            {
                Fecha = DateTime.Today
            };

            return View("~/Views/Gasto/Create.cshtml", model);
        }

        // POST: Gasto/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Gasto model)
        {
            if (model.ID_Categoria == null || model.ID_Categoria <= 0)
            {
                ModelState.AddModelError("ID_Categoria", "Debes seleccionar una categoría.");
            }

            if (model.Monto == null || model.Monto <= 0)
            {
                ModelState.AddModelError("Monto", "Debes ingresar un monto mayor a 0.");
            }

            if (model.Fecha == null)
            {
                model.Fecha = DateTime.Today;
            }

            if (ModelState.IsValid)
            {
                db.Gastos.Add(model);
                db.SaveChanges();

                TempData["Success"] = "El gasto se registró correctamente.";
                return RedirectToAction("Index", "Gasto");
            }

            CargarCategorias(model.ID_Categoria);
            return View("~/Views/Gasto/Create.cshtml", model);
        }

        private void CargarCategorias(int? categoriaSeleccionada)
        {
            var categorias = db.CategoriasFinancieras
                               .Where(c => c.Tipo == "Gasto")
                               .OrderBy(c => c.Nombre)
                               .ToList();

            ViewBag.ID_Categoria = new SelectList(categorias, "ID_Categoria", "Nombre", categoriaSeleccionada);
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