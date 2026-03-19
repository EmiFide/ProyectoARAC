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
                .ToList();

            return View(lista);
        }

        public ActionResult Create()
        {
            ViewBag.ID_Voluntario = new SelectList(db.Voluntarios.Where(v => v.Estado), "ID_Voluntario", "Nombre");
            ViewBag.ID_Tarea = new SelectList(db.TareasVoluntariado.Where(t => t.Estado), "ID_Tarea", "Titulo");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ParticipacionVoluntario model)
        {
            if (ModelState.IsValid)
            {
                db.ParticipacionesVoluntario.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_Voluntario = new SelectList(db.Voluntarios.Where(v => v.Estado), "ID_Voluntario", "Nombre", model.ID_Voluntario);
            ViewBag.ID_Tarea = new SelectList(db.TareasVoluntariado.Where(t => t.Estado), "ID_Tarea", "Titulo", model.ID_Tarea);
            return View(model);
        }
    }
}