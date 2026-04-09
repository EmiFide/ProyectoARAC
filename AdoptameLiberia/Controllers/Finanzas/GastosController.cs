using System.Linq;
using System.Web.Mvc;
using AdoptameLiberia.Models;
using AdoptameLiberia.Models.Finanzas;

namespace AdoptameLiberia.Controllers.Finanzas
{
    public class GastosController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var gastos = db.Gastos.ToList();
            return View("~/Views/Gasto/Index.cshtml", gastos);
        }

        public ActionResult Create()
        {
            ViewBag.ID_Categoria = new SelectList(
                db.CategoriasFinancieras.Where(c => c.Tipo == "Gasto"),
                "ID_Categoria",
                "Nombre"
            );

            return View("~/Views/Gasto/Create.cshtml");
        }

        [HttpPost]
        public ActionResult Create(Gasto model)
        {
            if (ModelState.IsValid)
            {
                db.Gastos.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }
    }
}