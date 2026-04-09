using AdoptameLiberia.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AdoptameLiberia.Controllers
{
    public class AdopcionesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Adopciones
        public ActionResult Index()
        {
            var adopcion = db.Adopcion.Include(a => a.Animal).Include(a => a.Solicitud);
            return View(adopcion.ToList());
        }

        // GET: Adopciones/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adopcion adopcion = db.Adopcion.Find(id);
            if (adopcion == null)
            {
                return HttpNotFound();
            }
            return View(adopcion);
        }

        // GET: Adopciones/Create
        public ActionResult Create()
        {
            ViewBag.ID_Animal = new SelectList(db.Animals, "ID_Animal", "Nombre_Animal");
            ViewBag.ID_Solicitud = new SelectList(db.SolicitudAdopcion, "ID_Solicitud", "ID_Usuario");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Adopcion,ID_Solicitud,ID_Animal,Fecha_Adopcion,Estado_Adopcion,Seguimiento_Inicial")] Adopcion adopcion)
        {
            if (ModelState.IsValid)
            {
                db.Adopcion.Add(adopcion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Animal = new SelectList(db.Animals, "ID_Animal", "Nombre_Animal", adopcion.ID_Animal);
            ViewBag.ID_Solicitud = new SelectList(db.SolicitudAdopcion, "ID_Solicitud", "ID_Usuario", adopcion.ID_Solicitud);
            return View(adopcion);
        }

        // GET: Adopciones/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adopcion adopcion = db.Adopcion.Find(id);
            if (adopcion == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_Animal = new SelectList(db.Animals, "ID_Animal", "Nombre_Animal", adopcion.ID_Animal);
            ViewBag.ID_Solicitud = new SelectList(db.SolicitudAdopcion, "ID_Solicitud", "ID_Usuario", adopcion.ID_Solicitud);
            return View(adopcion);
        }

        // POST: Adopciones/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Adopcion,ID_Solicitud,ID_Animal,Fecha_Adopcion,Estado_Adopcion,Seguimiento_Inicial")] Adopcion adopcion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adopcion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_Animal = new SelectList(db.Animals, "ID_Animal", "Nombre_Animal", adopcion.ID_Animal);
            ViewBag.ID_Solicitud = new SelectList(db.SolicitudAdopcion, "ID_Solicitud", "ID_Usuario", adopcion.ID_Solicitud);
            return View(adopcion);
        }

        // GET: Adopciones/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adopcion adopcion = db.Adopcion.Find(id);
            if (adopcion == null)
            {
                return HttpNotFound();
            }
            return View(adopcion);
        }

        // POST: Adopciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Adopcion adopcion = db.Adopcion.Find(id);
            db.Adopcion.Remove(adopcion);
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
