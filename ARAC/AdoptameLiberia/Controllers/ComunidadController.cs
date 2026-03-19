using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AdoptameLiberia.Models.Comunidad;
using AdoptameLiberia.Models.Donaciones;

namespace AdoptameLiberia.Controllers
{
    [Authorize]
    public class ComunidadController : Controller
    {
        private ARACDbContext db = new ARACDbContext();

        public ActionResult Index()
        {
            var publicaciones = db.PublicacionesComunidad
                .Where(p => p.Estado)
                .OrderByDescending(p => p.Fecha)
                .ToList();

            return View(publicaciones);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PublicacionComunidad model)
        {
            if (ModelState.IsValid)
            {
                model.ID_Usuario = 1;
                model.ID_Categoria = 1;
                model.Fecha = DateTime.Now;
                model.Estado = true;

                db.PublicacionesComunidad.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var publicacion = db.PublicacionesComunidad.FirstOrDefault(p => p.ID_Publicacion == id);
            if (publicacion == null)
                return HttpNotFound();

            ViewBag.Comentarios = db.ComentariosPublicacion
                .Where(c => c.ID_Publicacion == id && c.Estado)
                .OrderBy(c => c.Fecha)
                .ToList();

            return View(publicacion);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Comentar(int idPublicacion, string contenido)
        {
            if (!string.IsNullOrWhiteSpace(contenido))
            {
                var comentario = new ComentarioPublicacion
                {
                    ID_Publicacion = idPublicacion,
                    ID_Usuario = 1,
                    Contenido = contenido,
                    Fecha = DateTime.Now,
                    Estado = true
                };

                db.ComentariosPublicacion.Add(comentario);
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = idPublicacion });
        }
    }
}