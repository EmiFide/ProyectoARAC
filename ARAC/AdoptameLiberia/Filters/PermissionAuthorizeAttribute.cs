using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace AdoptameLiberia.Filters
{
    // Autoriza por permisos (lectura/escritura) asociados al/los roles del usuario.
    // Uso:
    // [PermissionAuthorize(Module = "Animales", RequireWrite = true)]
    public class PermissionAuthorizeAttribute : AuthorizeAttribute
    {
        public string Module { get; set; }
        public bool RequireWrite { get; set; } = false;

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            // Primero valida autenticación estándar
            if (!base.AuthorizeCore(httpContext)) return false;

            var user = httpContext.User;
            if (user == null || !user.Identity.IsAuthenticated) return false;

            // Administrador siempre pasa
            if (user.IsInRole("Administrador")) return true;

            if (string.IsNullOrWhiteSpace(Module))
            {
                // Si no se especifica módulo, se comporta como [Authorize]
                return true;
            }

            var userId = user.Identity.GetUserId();

            using (var db = new AdoptameLiberia.Models.ApplicationDbContext())
            {
                var roleIds = db.Users
                    .Where(u => u.Id == userId)
                    .SelectMany(u => u.Roles.Select(r => r.RoleId))
                    .ToList();

                if (!roleIds.Any()) return false;

                var query = from rp in db.RoleModulePermissions
                            join m in db.Modules on rp.ModuleId equals m.ModuleId
                            where roleIds.Contains(rp.RoleId) && m.Name == Module
                            select rp;

                if (RequireWrite)
                {
                    return query.Any(p => p.CanWrite);
                }

                // Lectura: CanRead o CanWrite
                return query.Any(p => p.CanRead || p.CanWrite);
            }
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Si está autenticado pero no autorizado -> 403
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpStatusCodeResult(403);
                return;
            }
            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}
