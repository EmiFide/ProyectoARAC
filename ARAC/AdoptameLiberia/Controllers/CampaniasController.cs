using AdoptameLiberia.Models.Campanias;
using AdoptameLiberia.Models.Donaciones;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AdoptameLiberia.Controllers
{
    [Authorize]
    public class CampaniasController : Controller
    {
        private ARACDbContext db = new ARACDbContext();

        public ActionResult Index()
        {
            var campanias = db.CampaniasCastracion
                .OrderBy(c => c.Fecha)
                .ToList();

            var cuposDisponibles = db.InscripcionesCastracion
                .GroupBy(i => i.CampaniaCastracionId)
                .ToDictionary(g => g.Key, g => g.Count());

            ViewBag.CuposDisponibles = cuposDisponibles;

            return View(campanias);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
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

        [Authorize(Roles = "Administrador")]
        public ActionResult VerInscripciones(int id)
        {
            var inscripciones = db.InscripcionesCastracion
                .Include(i => i.Animal)
                .Where(i => i.CampaniaCastracionId == id)
                .ToList();

            ViewBag.CampaniaId = id;
            return View(inscripciones);
        }

        public ActionResult Inscribir(int id)
        {
            var campania = db.CampaniasCastracion.FirstOrDefault(c => c.Id == id);

            if (campania == null)
            {
                return HttpNotFound();
            }

            ViewBag.CampaniaNombre = campania.Nombre;
            ViewBag.CampaniaFecha = campania.Fecha;
            ViewBag.CampaniaLugar = campania.Lugar;
            ViewBag.CampaniaId = campania.Id;

            ViewBag.AnimalId = new SelectList(
                db.Animales.OrderBy(a => a.Nombre_Animal).ToList(),
                "ID_Animal",
                "Nombre_Animal"
            );

            var model = new InscripcionCastracion
            {
                CampaniaCastracionId = campania.Id
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Inscribir(InscripcionCastracion model)
        {
            var campania = db.CampaniasCastracion.FirstOrDefault(c => c.Id == model.CampaniaCastracionId);

            if (campania == null)
            {
                return HttpNotFound();
            }

            ViewBag.CampaniaNombre = campania.Nombre;
            ViewBag.CampaniaFecha = campania.Fecha;
            ViewBag.CampaniaLugar = campania.Lugar;
            ViewBag.CampaniaId = campania.Id;

            ViewBag.AnimalId = new SelectList(
                db.Animales.OrderBy(a => a.Nombre_Animal).ToList(),
                "ID_Animal",
                "Nombre_Animal",
                model.AnimalId
            );

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdAsp = User.Identity.GetUserId();
            var usuario = db.Usuarios.FirstOrDefault(u => u.IdAspNetUser == userIdAsp);

            if (usuario == null)
            {
                ModelState.AddModelError("", "No se encontró el usuario actual en la tabla Usuario.");
                return View(model);
            }

            var yaInscrito = db.InscripcionesCastracion.Any(i =>
                i.CampaniaCastracionId == model.CampaniaCastracionId &&
                i.AnimalId == model.AnimalId
            );

            if (yaInscrito)
            {
                ModelState.AddModelError("", "Esta mascota ya está inscrita en esta campaña.");
                return View(model);
            }

            var totalInscritos = db.InscripcionesCastracion.Count(i => i.CampaniaCastracionId == model.CampaniaCastracionId);

            if (totalInscritos >= campania.Cupos)
            {
                ModelState.AddModelError("", "Ya no hay cupos disponibles para esta campaña.");
                return View(model);
            }

            var nuevaInscripcion = new InscripcionCastracion
            {
                CampaniaCastracionId = model.CampaniaCastracionId,
                AnimalId = model.AnimalId,
                IdUsuario = usuario.ID_Usuario,
                VeterinarioAsignado = null,
                Resultado = null
            };

            db.InscripcionesCastracion.Add(nuevaInscripcion);
            db.SaveChanges();

            TempData["Success"] = "La mascota fue inscrita correctamente en la campaña.";
            return RedirectToAction("MisCampanias");
        }

        public ActionResult MisCampanias()
        {
            var userIdAsp = User.Identity.GetUserId();
            var usuario = db.Usuarios.FirstOrDefault(u => u.IdAspNetUser == userIdAsp);

            if (usuario == null)
            {
                return View(new List<InscripcionCastracion>());
            }

            var inscripciones = db.InscripcionesCastracion
                .Include(i => i.Animal)
                .Include(i => i.Campania)
                .Where(i => i.IdUsuario == usuario.ID_Usuario)
                .OrderByDescending(i => i.Id)
                .ToList();

            return View(inscripciones);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult RegistrarResultado(int id)
        {
            var inscripcion = db.InscripcionesCastracion
                .Include(i => i.Animal)
                .FirstOrDefault(i => i.Id == id);

            if (inscripcion == null)
            {
                return HttpNotFound();
            }

            return View(inscripcion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult RegistrarResultado(InscripcionCastracion model)
        {
            var inscripcion = db.InscripcionesCastracion
                .FirstOrDefault(i => i.Id == model.Id);

            if (inscripcion == null)
            {
                return HttpNotFound();
            }

            inscripcion.Resultado = model.Resultado;
            db.SaveChanges();

            return RedirectToAction("VerInscripciones", new { id = inscripcion.CampaniaCastracionId });
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult AsignarVeterinario(int id)
        {
            var inscripcion = db.InscripcionesCastracion
                .Include(i => i.Animal)
                .FirstOrDefault(i => i.Id == id);

            if (inscripcion == null)
            {
                return HttpNotFound();
            }

            return View(inscripcion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult AsignarVeterinario(InscripcionCastracion model)
        {
            var inscripcion = db.InscripcionesCastracion
                .FirstOrDefault(i => i.Id == model.Id);

            if (inscripcion == null)
            {
                return HttpNotFound();
            }

            inscripcion.VeterinarioAsignado = model.VeterinarioAsignado;
            db.SaveChanges();

            return RedirectToAction("VerInscripciones", new { id = inscripcion.CampaniaCastracionId });
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