using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AdoptameLiberia.Models.Donaciones;
using AdoptameLiberia.Models.Voluntariado;

namespace AdoptameLiberia.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ParticipacionesVoluntarioController : Controller
    {
        private ARACDbContext db = new ARACDbContext();

        public ActionResult Index()
        {
            var lista = db.ParticipacionesVoluntario
                .Include(p => p.Voluntario)
                .Include(p => p.Tarea)
                .OrderByDescending(p => p.Fecha_Registro)
                .ToList();

            return View(lista);
        }

        public ActionResult Create(int? idVoluntario)
        {
            var voluntariosAsignadosIds = db.ParticipacionesVoluntario
                .Select(p => p.ID_Voluntario)
                .Distinct()
                .ToList();

            var voluntariosDisponibles = db.Voluntarios
                .Where(v => v.Estado && !voluntariosAsignadosIds.Contains(v.ID_Voluntario))
                .OrderBy(v => v.Nombre)
                .ToList();

            ViewBag.ID_Voluntario = new SelectList(
                voluntariosDisponibles,
                "ID_Voluntario",
                "Nombre",
                idVoluntario
            );

            ViewBag.ID_Tarea = new SelectList(
                db.TareasVoluntariado.Where(t => t.Estado).OrderBy(t => t.Titulo),
                "ID_Tarea",
                "Titulo"
            );

            var model = new ParticipacionVoluntario
            {
                ID_Voluntario = idVoluntario ?? 0,
                Asistio = false,
                Fecha_Registro = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ParticipacionVoluntario model)
        {
            if (ModelState.IsValid)
            {
                model.Fecha_Registro = DateTime.Now;
                db.ParticipacionesVoluntario.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index", "Voluntarios");
            }

            var voluntariosAsignadosIds = db.ParticipacionesVoluntario
                .Select(p => p.ID_Voluntario)
                .Distinct()
                .ToList();

            var voluntariosDisponibles = db.Voluntarios
                .Where(v => v.Estado && !voluntariosAsignadosIds.Contains(v.ID_Voluntario))
                .OrderBy(v => v.Nombre)
                .ToList();

            if (model.ID_Voluntario > 0 && !voluntariosDisponibles.Any(v => v.ID_Voluntario == model.ID_Voluntario))
            {
                var voluntarioActual = db.Voluntarios.Find(model.ID_Voluntario);
                if (voluntarioActual != null)
                {
                    voluntariosDisponibles.Add(voluntarioActual);
                }
            }

            ViewBag.ID_Voluntario = new SelectList(
                voluntariosDisponibles.OrderBy(v => v.Nombre),
                "ID_Voluntario",
                "Nombre",
                model.ID_Voluntario
            );

            ViewBag.ID_Tarea = new SelectList(
                db.TareasVoluntariado.Where(t => t.Estado).OrderBy(t => t.Titulo),
                "ID_Tarea",
                "Titulo",
                model.ID_Tarea
            );

            return View(model);
        }
    }
}