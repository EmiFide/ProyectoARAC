using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AdoptameLiberia.Models.Donaciones;
using AdoptameLiberia.Models.Voluntariado;

namespace AdoptameLiberia.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class TareasVoluntariadoController : Controller
    {
        private ARACDbContext db = new ARACDbContext();

        public ActionResult Index()
        {
            return RedirectToAction("Index", "Voluntarios");
        }

        public ActionResult Create()
        {
            var model = new TareaVoluntariado
            {
                Fecha = DateTime.Today,
                Fecha_Registro = DateTime.Now,
                Estado = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TareaVoluntariado tarea)
        {
            if (ModelState.IsValid)
            {
                tarea.Fecha_Registro = DateTime.Now;
                db.TareasVoluntariado.Add(tarea);
                db.SaveChanges();
                return RedirectToAction("Index", "Voluntarios");
            }

            return View(tarea);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var tarea = db.TareasVoluntariado.Find(id);

            if (tarea == null)
                return HttpNotFound();

            return View(tarea);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TareaVoluntariado tarea)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tarea).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Voluntarios");
            }

            return View(tarea);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var tarea = db.TareasVoluntariado.Find(id);

            if (tarea == null)
                return HttpNotFound();

            var voluntariosAsignados = db.ParticipacionesVoluntario
                .Include(p => p.Voluntario)
                .Where(p => p.ID_Tarea == tarea.ID_Tarea)
                .OrderByDescending(p => p.Fecha_Registro)
                .ToList();

            ViewBag.VoluntariosAsignados = voluntariosAsignados;

            return View(tarea);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var tarea = db.TareasVoluntariado.Find(id);

            if (tarea == null)
                return HttpNotFound();

            return View(tarea);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var participaciones = db.ParticipacionesVoluntario
                .Where(p => p.ID_Tarea == id)
                .ToList();

            if (participaciones.Any())
            {
                db.ParticipacionesVoluntario.RemoveRange(participaciones);
            }

            var tarea = db.TareasVoluntariado.Find(id);

            if (tarea == null)
                return HttpNotFound();

            db.TareasVoluntariado.Remove(tarea);
            db.SaveChanges();
            return RedirectToAction("Index", "Voluntarios");
        }

        public ActionResult CambiarEstado(int id)
        {
            var tarea = db.TareasVoluntariado.Find(id);

            if (tarea == null)
                return HttpNotFound();

            tarea.Estado = !tarea.Estado;
            db.SaveChanges();

            return RedirectToAction("Index", "Voluntarios");
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