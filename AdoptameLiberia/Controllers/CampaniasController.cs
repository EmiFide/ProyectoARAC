using AdoptameLiberia.Models;
using AdoptameLiberia.Models.Campanias;
using AdoptameLiberia.Models.Donaciones;
using AdoptameLiberia.Models.Mascotas;
using Microsoft.AspNet.Identity;
using System;
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

            var cuposPorCampania = db.InscripcionesCastracion
                .GroupBy(i => i.CampaniaCastracionId)
                .ToDictionary(g => g.Key, g => g.Count());

            var cuposDisponibles = new Dictionary<int, int>();

            foreach (var campania in campanias)
            {
                var inscritos = cuposPorCampania.ContainsKey(campania.Id) ? cuposPorCampania[campania.Id] : 0;
                var disponibles = campania.Cupos - inscritos;

                if (disponibles < 0)
                {
                    disponibles = 0;
                }

                cuposDisponibles[campania.Id] = disponibles;
            }

            ViewBag.CuposDisponibles = cuposDisponibles;
            ViewBag.TotalCampanias = campanias.Count;
            ViewBag.CampaniasDisponibles = cuposDisponibles.Values.Count(x => x > 0);

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

            CargarMascotasPermitidas(null);

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

            CargarMascotasPermitidas(model.AnimalId);

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

            if (!UsuarioPuedeUsarMascota(model.AnimalId))
            {
                ModelState.AddModelError("", "No puedes inscribir una mascota que no pertenece a tu usuario.");
                return View(model);
            }

            var mascotaYaRegistradaEnAlgunaCampania = db.InscripcionesCastracion.Any(i =>
    i.AnimalId == model.AnimalId
);

            if (mascotaYaRegistradaEnAlgunaCampania)
            {
                ModelState.AddModelError("", "Esta mascota ya fue registrada en una campaña y no puede volver a inscribirse.");
                CargarMascotasPermitidas(model.AnimalId);
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

        private void CargarMascotasPermitidas(int? animalSeleccionado)
        {
            var userIdAsp = User.Identity.GetUserId();
            var esAdmin = User.IsInRole("Administrador");

            List<AnimalModel> mascotas;

            if (esAdmin)
            {
                mascotas = db.Database.SqlQuery<AnimalModel>(@"
            SELECT 
                a.ID_Animal,
                a.Nombre_Animal,
                a.ID_Raza,
                a.ID_TipoAnimal,
                a.Edad,
                a.Sexo,
                a.Tamano,
                a.Peso,
                a.Descripcion,
                a.Estado
            FROM Animal a
            WHERE a.Estado = 'Disponible'
              AND NOT EXISTS (
                    SELECT 1
                    FROM InscripcionesCastracion ic
                    WHERE ic.AnimalId = a.ID_Animal
              )
            ORDER BY a.Nombre_Animal
        ").ToList();
            }
            else
            {
                mascotas = db.Database.SqlQuery<AnimalModel>(@"
            SELECT 
                a.ID_Animal,
                a.Nombre_Animal,
                a.ID_Raza,
                a.ID_TipoAnimal,
                a.Edad,
                a.Sexo,
                a.Tamano,
                a.Peso,
                a.Descripcion,
                a.Estado
            FROM Animal a
            WHERE a.Estado = 'Disponible'
              AND a.UsuarioRegistroId = @p0
              AND NOT EXISTS (
                    SELECT 1
                    FROM InscripcionesCastracion ic
                    WHERE ic.AnimalId = a.ID_Animal
              )
            ORDER BY a.Nombre_Animal
        ", userIdAsp).ToList();
            }

            ViewBag.AnimalId = new SelectList(mascotas, "ID_Animal", "Nombre_Animal", animalSeleccionado);
        }

        private bool UsuarioPuedeUsarMascota(int animalId)
        {
            var userIdAsp = User.Identity.GetUserId();
            var esAdmin = User.IsInRole("Administrador");

            if (esAdmin)
            {
                return db.Database.SqlQuery<int>(@"
                    SELECT COUNT(1)
                    FROM Animal
                    WHERE ID_Animal = @p0
                ", animalId).FirstOrDefault() > 0;
            }

            return db.Database.SqlQuery<int>(@"
                SELECT COUNT(1)
                FROM Animal
                WHERE ID_Animal = @p0
                  AND UsuarioRegistroId = @p1
            ", animalId, userIdAsp).FirstOrDefault() > 0;
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