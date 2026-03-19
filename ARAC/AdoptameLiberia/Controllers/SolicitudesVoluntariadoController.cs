using System;
using System.Web.Mvc;
using AdoptameLiberia.Models.Donaciones;
using AdoptameLiberia.Models.Voluntariado;

namespace AdoptameLiberia.Controllers
{
    [Authorize]
    public class SolicitudesVoluntariadoController : Controller
    {
        private ARACDbContext db = new ARACDbContext();

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Voluntario model)
        {
            if (ModelState.IsValid)
            {
                model.Estado = false;
                model.Fecha_Registro = DateTime.Now;

                db.Voluntarios.Add(model);
                db.SaveChanges();

                TempData["Mensaje"] = "Solicitud enviada correctamente.";
                return RedirectToAction("Create");
            }

            return View(model);
        }
    }
}