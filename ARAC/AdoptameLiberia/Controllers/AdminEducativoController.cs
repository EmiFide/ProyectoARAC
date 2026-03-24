using AdoptameLiberia.Models.Donaciones;
using AdoptameLiberia.Models.Educativo.VM;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AdoptameLiberia.Controllers
{
    public class AdminEducativoController : Controller
    {
        private readonly ARACDbContext db = new ARACDbContext();

        public ActionResult Index(string tipoContenido, string tema, string textoBusqueda)
        {
            var query = db.ContenidoEducativo
                .Where(x => x.Activo)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tipoContenido))
                query = query.Where(x => x.TipoContenido == tipoContenido);

            if (!string.IsNullOrWhiteSpace(tema))
                query = query.Where(x => x.Tema == tema);

            if (!string.IsNullOrWhiteSpace(textoBusqueda))
                query = query.Where(x =>
                    x.Titulo.Contains(textoBusqueda) ||
                    x.Descripcion.Contains(textoBusqueda));

            var model = new EducativoFiltro
            {
                TipoContenido = tipoContenido,
                Tema = tema,
                TextoBusqueda = textoBusqueda,
                Resultados = query
                    .OrderBy(x => x.Tema)
                    .ThenBy(x => x.Titulo)
                    .ToList(),
                Tipos = db.ContenidoEducativo
                    .Where(x => x.Activo)
                    .Select(x => x.TipoContenido)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList()
                    .Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    }),
                Temas = db.ContenidoEducativo
                    .Where(x => x.Activo)
                    .Select(x => x.Tema)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList()
                    .Select(x => new SelectListItem
                    {
                        Value = x,
                        Text = x
                    })
            };

            return View(model);
        }

        public ActionResult Detalle(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var recurso = db.ContenidoEducativo
                .FirstOrDefault(x => x.IdContenidoEducativo == id && x.Activo);

            if (recurso == null)
                return HttpNotFound();

            bool esFavorito = false;

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                esFavorito = db.ContenidoFavorito.Any(x =>
                    x.UsuarioId == userId &&
                    x.IdContenidoEducativo == recurso.IdContenidoEducativo);
            }

            ViewBag.EsFavorito = esFavorito;
            return View(recurso);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}