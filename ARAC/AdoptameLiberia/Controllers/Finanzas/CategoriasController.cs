using System.Linq;
using System.Web.Mvc;
using AdoptameLiberia.Models;
using AdoptameLiberia.Models.Finanzas;

namespace AdoptameLiberia.Controllers.Finanzas
{
    public class CategoriasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var lista = db.CategoriasFinancieras.ToList();

            return View("~/Views/Categoria/Index.cshtml", lista);
        }

        public ActionResult Create()
        {
            return View(new CategoriaFinanciera());
        }

        [HttpPost]
        public ActionResult Create(CategoriaFinanciera model)
        {
            if (ModelState.IsValid)
            {
                db.CategoriasFinancieras.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}