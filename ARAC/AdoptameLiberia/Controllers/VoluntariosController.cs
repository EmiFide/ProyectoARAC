using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AdoptameLiberia.Models.Donaciones;
using AdoptameLiberia.Models.Voluntariado;

namespace AdoptameLiberia.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class VoluntariosController : Controller
    {
        private ARACDbContext db = new ARACDbContext();

        public ActionResult Index()
        {
            return View(db.Voluntarios.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Voluntario voluntario)
        {
            if (ModelState.IsValid)
            {
                db.Voluntarios.Add(voluntario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(voluntario);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            Voluntario voluntario = db.Voluntarios.Find(id);

            if (voluntario == null)
                return HttpNotFound();

            return View(voluntario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Voluntario voluntario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(voluntario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(voluntario);
        }

        public ActionResult CambiarEstado(int id)
        {
            var voluntario = db.Voluntarios.Find(id);

            if (voluntario == null)
                return HttpNotFound();

            voluntario.Estado = !voluntario.Estado;
            db.SaveChanges();

            return RedirectToAction("Index");
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