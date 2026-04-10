using AdoptameLiberia.Models;
using AdoptameLiberia.Models.Campanias;
using AdoptameLiberia.Models.Donaciones;
using AdoptameLiberia.Models.Mascotas;
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

        // 🔥 MÉTODO CLAVE (NUEVO)
        private int ObtenerIdUsuarioActual()
        {
            var userIdAsp = User.Identity.GetUserId();

            return db.Usuarios
                .Where(u => u.IdAspNetUser == userIdAsp)
                .Select(u => u.ID_Usuario)
                .FirstOrDefault();
        }

        public ActionResult AsignarVeterinario(int id)
        {
            var inscripcion = db.InscripcionesCastracion
                .Include(i => i.Animal)
                .FirstOrDefault(i => i.Id == id);

            if (inscripcion == null)
                return HttpNotFound();

            return View(inscripcion); // 🔥 IMPORTANTE
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarVeterinario(InscripcionCastracion model)
        {
            var inscripcion = db.InscripcionesCastracion
                .FirstOrDefault(i => i.Id == model.Id);

            if (inscripcion == null)
                return HttpNotFound();

            // 🔥 Guardar datos
            inscripcion.VeterinarioAsignado = model.VeterinarioAsignado;
            inscripcion.Resultado = model.Resultado;

            db.SaveChanges();

            return RedirectToAction("VerInscripciones", new { id = inscripcion.CampaniaCastracionId });
        }

        public ActionResult RegistrarResultado(int id)
        {
            var inscripcion = db.InscripcionesCastracion
                .Include(i => i.Animal)
                .FirstOrDefault(i => i.Id == id);

            if (inscripcion == null)
                return HttpNotFound();

            return View(inscripcion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarResultado(InscripcionCastracion model)
        {
            var inscripcion = db.InscripcionesCastracion
                .FirstOrDefault(i => i.Id == model.Id);

            if (inscripcion == null)
                return HttpNotFound();

            // 🔥 SOLO ACTUALIZAS EL RESULTADO
            inscripcion.Resultado = model.Resultado;

            db.SaveChanges();

            return RedirectToAction("VerInscripciones", new { id = inscripcion.CampaniaCastracionId });
        }


        public ActionResult VerInscripciones(int id)
        {
            var inscripciones = db.InscripcionesCastracion
                .Include(i => i.Animal)
                .Include(i => i.Campania)
                .Where(i => i.CampaniaCastracionId == id)
                .ToList();

            return View(inscripciones);
        }

        public ActionResult Index()
        {
            var campanias = db.CampaniasCastracion
                .OrderBy(c => c.Fecha)
                .ToList();

            return View(campanias);
        }

        public ActionResult Inscribir(int id)
        {
            var campania = db.CampaniasCastracion.FirstOrDefault(c => c.Id == id);

            if (campania == null)
                return HttpNotFound();

            ViewBag.CampaniaNombre = campania.Nombre;
            ViewBag.CampaniaFecha = campania.Fecha;
            ViewBag.CampaniaLugar = campania.Lugar;
            ViewBag.CampaniaId = campania.Id;

            CargarMascotasPermitidas(null);

            return View(new InscripcionCastracion
            {
                CampaniaCastracionId = id
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Inscribir(InscripcionCastracion model)
        {
            var campania = db.CampaniasCastracion
                .FirstOrDefault(c => c.Id == model.CampaniaCastracionId);

            if (campania == null)
                return HttpNotFound();

            // 🔥 ESTO FALTABA
            ViewBag.CampaniaNombre = campania.Nombre;
            ViewBag.CampaniaFecha = campania.Fecha;
            ViewBag.CampaniaLugar = campania.Lugar;
            ViewBag.CampaniaId = campania.Id;

            CargarMascotasPermitidas(model.AnimalId);

            if (!ModelState.IsValid)
                return View(model);

            int userId = ObtenerIdUsuarioActual();

            db.InscripcionesCastracion.Add(new InscripcionCastracion
            {
                CampaniaCastracionId = model.CampaniaCastracionId,
                AnimalId = model.AnimalId,
                IdUsuario = userId
            });

            db.SaveChanges();

            return RedirectToAction("MisCampanias");
        }

        public ActionResult MisCampanias()
        {
            int userId = ObtenerIdUsuarioActual();

            var inscripciones = db.InscripcionesCastracion
                .Include(i => i.Animal)
                .Include(i => i.Campania)
                .Where(i => i.IdUsuario == userId)
                .ToList();

            return View(inscripciones);
        }

        // 🔥 AQUÍ ESTÁ EL CAMBIO IMPORTANTE
        private void CargarMascotasPermitidas(int? animalSeleccionado)
        {
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
                        a.Estado,
                        a.UsuarioRegistroId
                    FROM Animal a
                    WHERE NOT EXISTS (
                        SELECT 1
                        FROM InscripcionesCastracion ic
                        WHERE ic.AnimalId = a.ID_Animal
                    )
                    ORDER BY a.Nombre_Animal
                ").ToList();
            }
            else
            {
                int userId = ObtenerIdUsuarioActual(); // 🔥 CLAVE

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
                        a.Estado,
                        a.UsuarioRegistroId
                    FROM Animal a
                    WHERE a.ID_Animal IN (
                        SELECT ad.ID_Animal
                        FROM Adopcion ad
                        INNER JOIN Solicitud_Adopcion s 
                            ON ad.ID_Solicitud = s.ID_Solicitud
                        WHERE s.ID_Usuario = @p0
                    )
                    AND NOT EXISTS (
                        SELECT 1
                        FROM InscripcionesCastracion ic
                        WHERE ic.AnimalId = a.ID_Animal
                    )
                    ORDER BY a.Nombre_Animal
                ", userId).ToList();
            }

            ViewBag.AnimalId = new SelectList(mascotas, "ID_Animal", "Nombre_Animal", animalSeleccionado);
        }

        private bool UsuarioPuedeUsarMascota(int animalId)
        {
            if (User.IsInRole("Administrador"))
                return true;

            int userId = ObtenerIdUsuarioActual();

            return db.Database.SqlQuery<int>(@"
                SELECT COUNT(1)
                FROM Animal a
                WHERE a.ID_Animal = @p0
                AND a.ID_Animal IN (
                    SELECT ad.ID_Animal
                    FROM Adopcion ad
                    INNER JOIN Solicitud_Adopcion s 
                        ON ad.ID_Solicitud = s.ID_Solicitud
                    WHERE s.ID_Usuario = @p1
                )
            ", animalId, userId).FirstOrDefault() > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}