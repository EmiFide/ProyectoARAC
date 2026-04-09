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
    public class VoluntariosController : Controller
    {
        private ARACDbContext db = new ARACDbContext();

        public ActionResult Index()
        {
            var voluntariosAsignadosIds = db.ParticipacionesVoluntario
                .Select(p => p.ID_Voluntario)
                .Distinct()
                .ToList();

            var voluntariosPendientes = db.Voluntarios
                .Where(v => !voluntariosAsignadosIds.Contains(v.ID_Voluntario))
                .OrderByDescending(v => v.Fecha_Registro)
                .ToList();

            var tareas = db.TareasVoluntariado
                .OrderByDescending(t => t.Fecha)
                .ThenBy(t => t.Hora)
                .ToList();

            var asignacionesPorTarea = db.ParticipacionesVoluntario
                .GroupBy(p => p.ID_Tarea)
                .ToDictionary(g => g.Key, g => g.Count());

            ViewBag.TareasVoluntariado = tareas;
            ViewBag.AsignacionesPorTarea = asignacionesPorTarea;

            return View(voluntariosPendientes);
        }

        public ActionResult Create()
        {
            var model = new Voluntario
            {
                Estado = true,
                Fecha_Registro = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Voluntario voluntario)
        {
            if (ModelState.IsValid)
            {
                voluntario.Fecha_Registro = DateTime.Now;
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