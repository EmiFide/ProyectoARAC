using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AdoptameLiberia.Models.Campanias;
using AdoptameLiberia.Models.Donaciones;

namespace AdoptameLiberia.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CampaniasController : Controller
    {
        private ARACDbContext db = new ARACDbContext();

        private class AnimalComboItem
        {
            public int ID_Animal { get; set; }
            public string Nombre_Animal { get; set; }
        }

        private void CargarAnimalesDisponibles(int? animalSeleccionado = null)
        {
            var animales = db.Database.SqlQuery<AnimalComboItem>(
                @"SELECT ID_Animal, Nombre_Animal
                  FROM Animal
                  WHERE Estado = 'Disponible'"
            ).ToList();

            ViewBag.AnimalId = new SelectList(animales, "ID_Animal", "Nombre_Animal", animalSeleccionado);
        }

        public ActionResult Index()
        {
            return View(db.CampaniasCastracion.ToList());
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CampaniaCastracion campania)
        {
            if (ModelState.IsValid)
            {
                db.CampaniasCastracion.Add(campania);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(campania);
        }

        public ActionResult Inscribir(int id)
        {
            CargarAnimalesDisponibles();

            var model = new InscripcionCastracion
            {
                CampaniaCastracionId = id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Inscribir(InscripcionCastracion inscripcion)
        {
            CargarAnimalesDisponibles(inscripcion.AnimalId);

            if (!ModelState.IsValid)
                return View(inscripcion);

            var campania = db.CampaniasCastracion.Find(inscripcion.CampaniaCastracionId);

            if (campania == null)
                return HttpNotFound();

            if (campania.Cupos <= 0)
            {
                ModelState.AddModelError("", "No hay cupos disponibles.");
                return View(inscripcion);
            }

            db.InscripcionesCastracion.Add(inscripcion);
            campania.Cupos--;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult AsignarVeterinario(int id)
        {
            var inscripcion = db.InscripcionesCastracion.Find(id);

            if (inscripcion == null)
                return HttpNotFound();

            return View(inscripcion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarVeterinario(InscripcionCastracion model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var inscripcion = db.InscripcionesCastracion.Find(model.Id);

            if (inscripcion == null)
                return HttpNotFound();

            inscripcion.VeterinarioAsignado = model.VeterinarioAsignado;
            inscripcion.Resultado = model.Resultado;

            db.SaveChanges();

            return RedirectToAction("VerInscripciones", new { id = inscripcion.CampaniaCastracionId });
        }

        public ActionResult RegistrarResultado(int id)
        {
            var inscripcion = db.InscripcionesCastracion.Find(id);

            if (inscripcion == null)
                return HttpNotFound();

            return View(inscripcion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarResultado(InscripcionCastracion model)
        {
            var inscripcion = db.InscripcionesCastracion.Find(model.Id);

            if (inscripcion == null)
                return HttpNotFound();

            inscripcion.Resultado = model.Resultado;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult VerInscripciones(int id)
        {
            var inscripciones = db.InscripcionesCastracion
                .Where(i => i.CampaniaCastracionId == id)
                .Include(i => i.Animal)
                .ToList();

            ViewBag.CampaniaId = id;

            return View(inscripciones);
        }
    }
}