using AdoptameLiberia.Models;
using AdoptameLiberia.Models.Noticias;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace AdoptameLiberia.Controllers.Noticias
{
    [Authorize(Roles = "Administrador")]
    public class NoticiasController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View(db.Noticias.OrderByDescending(n => n.Fecha_Publicacion).ToList());
        }

        public ActionResult Create()
        {
            return View(new Noticia());
        }

        [HttpPost]
        public ActionResult Create(Noticia model)
        {
            model.ID_Usuario = User.Identity.GetUserId();
            model.Fecha_Publicacion = DateTime.Now;

            db.Noticias.Add(model);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}