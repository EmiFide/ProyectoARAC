using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AdoptameLiberia.Models.Donaciones;
using AdoptameLiberia.Models.Voluntariado;
using Microsoft.AspNet.Identity;

namespace AdoptameLiberia.Controllers
{
    [Authorize]
    public class SolicitudesVoluntariadoController : Controller
    {
        private ARACDbContext db = new ARACDbContext();

        private int? ObtenerIdUsuarioActual()
        {
            var correo = User.Identity.GetUserName();
            var aspNetUserId = User.Identity.GetUserId();

            var usuario = db.Usuarios.FirstOrDefault(u =>
                (u.Correo != null && u.Correo == correo) ||
                (u.IdAspNetUser != null && u.IdAspNetUser == aspNetUserId));

            return usuario != null ? (int?)usuario.ID_Usuario : null;
        }

        private Voluntario ObtenerVoluntarioActual()
        {
            var correo = User.Identity.GetUserName();
            var idUsuario = ObtenerIdUsuarioActual();

            Voluntario voluntario = null;

            if (idUsuario.HasValue)
            {
                voluntario = db.Voluntarios
                    .FirstOrDefault(v => v.ID_Usuario == idUsuario.Value);
            }

            if (voluntario == null && !string.IsNullOrWhiteSpace(correo))
            {
                var correoNormalizado = correo.Trim().ToLower();

                voluntario = db.Voluntarios
                    .FirstOrDefault(v => v.Correo != null && v.Correo.ToLower() == correoNormalizado);

                if (voluntario != null && idUsuario.HasValue && voluntario.ID_Usuario == null)
                {
                    voluntario.ID_Usuario = idUsuario.Value;
                    db.SaveChanges();
                }
            }

            return voluntario;
        }

        private List<ParticipacionVoluntario> ObtenerTareasAsignadas(int idVoluntario)
        {
            return db.ParticipacionesVoluntario
                .Include(p => p.Tarea)
                .Where(p => p.ID_Voluntario == idVoluntario)
                .OrderByDescending(p => p.ID_Participacion)
                .ToList();
        }

        private int ObtenerUltimaParticipacionAsignada(int idVoluntario)
        {
            return db.ParticipacionesVoluntario
                .Where(p => p.ID_Voluntario == idVoluntario)
                .Select(p => (int?)p.ID_Participacion)
                .DefaultIfEmpty(0)
                .Max() ?? 0;
        }

        private void MarcarNotificacionesComoVistas(int idVoluntario)
        {
            var ultimaParticipacion = ObtenerUltimaParticipacionAsignada(idVoluntario);
            Session["UltimaParticipacionVistaVoluntario"] = ultimaParticipacion;
        }

        public ActionResult Create()
        {
            var correoActual = User.Identity.GetUserName();
            var voluntarioActual = ObtenerVoluntarioActual();

            Voluntario model;

            if (voluntarioActual != null)
            {
                model = voluntarioActual;
            }
            else
            {
                model = new Voluntario
                {
                    Correo = correoActual,
                    Estado = false,
                    Fecha_Registro = DateTime.Now
                };
            }

            var tareasAsignadas = new List<ParticipacionVoluntario>();

            if (voluntarioActual != null)
            {
                tareasAsignadas = ObtenerTareasAsignadas(voluntarioActual.ID_Voluntario);

                // Al entrar a "Ser voluntario", se marcan como vistas
                MarcarNotificacionesComoVistas(voluntarioActual.ID_Voluntario);
            }
            else
            {
                Session["UltimaParticipacionVistaVoluntario"] = 0;
            }

            ViewBag.TareasAsignadas = tareasAsignadas;
            ViewBag.TotalTareasAsignadas = tareasAsignadas.Count;
            ViewBag.TotalTareasActivas = tareasAsignadas.Count(t => t.Tarea != null && t.Tarea.Estado);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Voluntario model)
        {
            var correoActual = User.Identity.GetUserName();
            var idUsuarioActual = ObtenerIdUsuarioActual();
            var voluntarioExistente = ObtenerVoluntarioActual();

            if (string.IsNullOrWhiteSpace(model.Correo))
            {
                model.Correo = correoActual;
            }

            if (ModelState.IsValid)
            {
                if (voluntarioExistente == null)
                {
                    model.Correo = correoActual;
                    model.ID_Usuario = idUsuarioActual;
                    model.Estado = false;
                    model.Fecha_Registro = DateTime.Now;

                    db.Voluntarios.Add(model);
                    db.SaveChanges();

                    TempData["Mensaje"] = "Solicitud enviada correctamente.";
                }
                else
                {
                    voluntarioExistente.Nombre = model.Nombre;
                    voluntarioExistente.Apellido1 = model.Apellido1;
                    voluntarioExistente.Apellido2 = model.Apellido2;
                    voluntarioExistente.Correo = correoActual;
                    voluntarioExistente.Telefono = model.Telefono;
                    voluntarioExistente.Disponibilidad = model.Disponibilidad;
                    voluntarioExistente.Habilidades = model.Habilidades;

                    if (idUsuarioActual.HasValue)
                    {
                        voluntarioExistente.ID_Usuario = idUsuarioActual.Value;
                    }

                    db.Entry(voluntarioExistente).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Mensaje"] = "Tu información de voluntariado fue actualizada correctamente.";
                }

                return RedirectToAction("Create");
            }

            var tareasAsignadas = new List<ParticipacionVoluntario>();

            if (voluntarioExistente != null)
            {
                tareasAsignadas = ObtenerTareasAsignadas(voluntarioExistente.ID_Voluntario);
                MarcarNotificacionesComoVistas(voluntarioExistente.ID_Voluntario);
            }

            ViewBag.TareasAsignadas = tareasAsignadas;
            ViewBag.TotalTareasAsignadas = tareasAsignadas.Count;
            ViewBag.TotalTareasActivas = tareasAsignadas.Count(t => t.Tarea != null && t.Tarea.Estado);

            return View(model);
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