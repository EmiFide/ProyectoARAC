using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AdoptameLiberia.Models;

namespace AdoptameLiberia.Controllers
{
    public class RazasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Razas
        public ActionResult Index(int? tipoAnimalId)
        {
            // Query base
            var razas = db.Razas
                          .Include(r => r.TipoAnimal)
                          .AsQueryable();

            // Filtro por tipo de animal
            if (tipoAnimalId.HasValue)
            {
                razas = razas.Where(r => r.ID_TipoAnimal == tipoAnimalId.Value);
            }

            // Combo de tipos (solo activos)
            ViewBag.TiposAnimal = new SelectList(
                db.TipoAnimals.Where(t => t.Estado),
                "ID_TipoAnimal",
                "Nombre_Tipo_Animal",
                tipoAnimalId
            );

            return View(razas.ToList());
        }
        // GET: Razas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Raza raza = db.Razas.Find(id);
            if (raza == null)
            {
                return HttpNotFound();
            }
            return View(raza);
        }

        // GET: Razas/Create
        public ActionResult Create()
        {
            ViewBag.ID_TipoAnimal = new SelectList(db.TipoAnimals, "ID_TipoAnimal", "Nombre_Tipo_Animal");
            return View();
        }

        // POST: Razas/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_Raza,NombreRaza,Descripcion,ID_TipoAnimal")] Raza raza)
        {
            bool existe = db.Razas.Any(r =>
                r.NombreRaza == raza.NombreRaza &&
                r.ID_TipoAnimal == raza.ID_TipoAnimal
            );

            if (existe)
            {
                ModelState.AddModelError("NombreRaza",
                    "Ya existe una raza con ese nombre para el tipo de animal seleccionado.");
            }

            if (ModelState.IsValid)
            {
                db.Razas.Add(raza);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_TipoAnimal = new SelectList(
                db.TipoAnimals,
                "ID_TipoAnimal",
                "Nombre_Tipo_Animal",
                raza.ID_TipoAnimal
            );

            return View(raza);
        }

        // GET: Razas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Raza raza = db.Razas.Find(id);
            if (raza == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_TipoAnimal = new SelectList(db.TipoAnimals, "ID_TipoAnimal", "Nombre_Tipo_Animal", raza.ID_TipoAnimal);
            return View(raza);
        }

        // POST: Razas/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_Raza,NombreRaza,Descripcion,ID_TipoAnimal")] Raza raza)
        {
            bool existe = db.Razas.Any(r =>
            r.NombreRaza.Trim().ToUpper() == raza.NombreRaza.Trim().ToUpper() &&
            r.ID_TipoAnimal == raza.ID_TipoAnimal
            );

            if (existe)
            {
                ModelState.AddModelError("NombreRaza",
                    "Ya existe otra raza con ese nombre para el tipo de animal seleccionado.");
            }
            if (ModelState.IsValid)
            {
                db.Entry(raza).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_TipoAnimal = new SelectList(db.TipoAnimals, "ID_TipoAnimal", "Nombre_Tipo_Animal", raza.ID_TipoAnimal);
            return View(raza);
        }

        // GET: Razas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Raza raza = db.Razas.Find(id);
            if (raza == null)
            {
                return HttpNotFound();
            }
            return View(raza);
        }

        // POST: Razas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Raza raza = db.Razas.Find(id);
            db.Razas.Remove(raza);
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
