using System;
using System.Linq;
using System.Web.Mvc;
using AdoptameLiberia.Models;
using AdoptameLiberia.Models.Noticias;

namespace AdoptameLiberia.Controllers
{
    public class NoticiaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // =========================
        // LISTAR NOTICIAS (PÚBLICO)
        // =========================
        public ActionResult Index()
        {
            var noticias = db.Noticias
                             .Where(n => n.Estado == true)
                             .OrderByDescending(n => n.Fecha_Publicacion)
                             .ToList();

            return View(noticias);
        }

        // =========================
        // CREAR NOTICIA (ADMIN)
        // =========================
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // =========================
        // GUARDAR NOTICIA
        // =========================
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult Create(Noticia model)
        {
            if (ModelState.IsValid)
            {
                model.Fecha_Publicacion = DateTime.Now;
                model.Estado = true;
                model.Likes = 0;

                db.Noticias.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // =========================
        // LIKE (1 vez por sesión)
        // =========================
        [HttpPost]
        public ActionResult Like(int id)
        {
            var key = "like_" + id;

            // 👇 evita doble click rápido
            if (Session[key] == null)
            {
                var noticia = db.Noticias.FirstOrDefault(n => n.ID_Noticia == id);

                if (noticia != null)
                {
                    noticia.Likes++;
                    db.SaveChanges();

                    Session[key] = true;
                }
            }

            return RedirectToAction("Index");
        }

        // =========================
        // DETALLE
        // =========================
        public ActionResult Details(int id)
        {
            var noticia = db.Noticias
                            .FirstOrDefault(n => n.ID_Noticia == id);

            if (noticia == null)
            {
                return HttpNotFound();
            }

            return View(noticia);
        }
    }
}