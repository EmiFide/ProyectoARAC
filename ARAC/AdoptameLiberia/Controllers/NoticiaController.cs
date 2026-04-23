using AdoptameLiberia.Models;
using AdoptameLiberia.Models.Noticias;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AdoptameLiberia.Controllers
{
    public class NoticiaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index(int page = 1)
        {
            const int pageSize = 6;

            var query = db.Noticias
                          .Where(n => n.Estado == true)
                          .OrderByDescending(n => n.Fecha_Publicacion);

            var totalNoticias = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalNoticias / pageSize);

            if (totalPages == 0)
            {
                totalPages = 1;
            }

            if (page < 1)
            {
                page = 1;
            }

            if (page > totalPages)
            {
                page = totalPages;
            }

            var noticias = query.Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(noticias);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Noticia model, HttpPostedFileBase ImagenFile)
        {
            if (ModelState.IsValid)
            {
                // 🔥 SUBIR IMAGEN
                if (ImagenFile != null && ImagenFile.ContentLength > 0)
                {
                    string nombreArchivo = Guid.NewGuid() + Path.GetExtension(ImagenFile.FileName);

                    string ruta = Server.MapPath("~/Content/img/noticias/");

                    // Crear carpeta si no existe
                    if (!Directory.Exists(ruta))
                    {
                        Directory.CreateDirectory(ruta);
                    }

                    string rutaCompleta = Path.Combine(ruta, nombreArchivo);

                    ImagenFile.SaveAs(rutaCompleta);

                    model.ImagenUrl = "/Content/img/noticias/" + nombreArchivo;
                }

                model.Fecha_Publicacion = DateTime.Now;
                model.Estado = true;
                model.Likes = 0;

                db.Noticias.Add(model);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Like(int id, int page = 1)
        {
            var key = "like_" + id;

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

            return RedirectToAction("Index", new { page = page });
        }

        public ActionResult Details(int id)
        {
            var noticia = db.Noticias.FirstOrDefault(n => n.ID_Noticia == id);

            if (noticia == null)
            {
                return HttpNotFound();
            }

            return View(noticia);
        }
    }
}