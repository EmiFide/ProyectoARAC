using System;
using System.Linq;
using System.Web.Mvc;
using AdoptameLiberia.Models;
using AdoptameLiberia.Models.Finanzas;

namespace AdoptameLiberia.Controllers.Finanzas
{
    [Authorize]
    public class CategoriaController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Categoria
        public ActionResult Index()
        {
            var lista = db.CategoriasFinancieras
                          .OrderBy(c => c.Nombre)
                          .ToList();

            return View("~/Views/Categoria/Index.cshtml", lista);
        }

        // GET: Categoria/Create
        public ActionResult Create()
        {
            var model = new CategoriaFinanciera
            {
                Tipo = "Gasto",
                Estado = true,
                FechaRegistro = DateTime.Now
            };

            return View("~/Views/Categoria/Create.cshtml", model);
        }

        // POST: Categoria/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CategoriaFinanciera model)
        {
            model.Tipo = "Gasto";

            if (string.IsNullOrWhiteSpace(model.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
            }

            var estadoValues = Request.Form.GetValues("Estado");
            model.Estado = estadoValues != null && estadoValues.Contains("true");

            if (!model.FechaRegistro.HasValue)
            {
                model.FechaRegistro = DateTime.Now;
            }

            if (ModelState.IsValid)
            {
                db.CategoriasFinancieras.Add(model);
                db.SaveChanges();

                TempData["Success"] = "Categoría registrada correctamente.";
                return RedirectToAction("Index", "Categoria");
            }

            return View("~/Views/Categoria/Create.cshtml", model);
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