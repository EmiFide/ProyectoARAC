using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AdoptameLiberia.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace AdoptameLiberia.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminUsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationUserManager UserManager => HttpContext.GetOwinContext().Get<ApplicationUserManager>();
        private ApplicationRoleManager RoleManager => HttpContext.GetOwinContext().Get<ApplicationRoleManager>();

        // HU-2.1 Criterio 2: muestra lista de usuarios registrados
        public ActionResult Index()
        {
            var users = db.Users
                .OrderBy(u => u.Email)
                .ToList()
                .Select(u => new UserListItemVM
                {
                    UserId = u.Id,
                    Email = u.Email,
                    Roles = UserManager.GetRoles(u.Id)
                })
                .ToList();

            return View(users);
        }

        // HU-2.1: asignar roles a un usuario
        public ActionResult AssignRoles(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return HttpNotFound();

            var user = db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null) return HttpNotFound();

            var userRoles = UserManager.GetRoles(user.Id);
            var allRoles = db.Roles.OrderBy(r => r.Name).ToList();

            var vm = new UserAssignRolesVM
            {
                UserId = user.Id,
                Email = user.Email,
                AvailableRoles = allRoles.Select(r => new RoleCheckboxVM
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    Selected = userRoles.Contains(r.Name)
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AssignRoles(UserAssignRolesVM model)
        {
            var user = db.Users.FirstOrDefault(u => u.Id == model.UserId);
            if (user == null) return HttpNotFound();

            var currentRoles = UserManager.GetRoles(user.Id);
            var selectedRoleNames = model.AvailableRoles
                .Where(r => r.Selected)
                .Select(r => r.RoleName)
                .ToList();

            // Quita los actuales
            if (currentRoles.Any())
            {
                var removeResult = await UserManager.RemoveFromRolesAsync(user.Id, currentRoles.ToArray());
                if (!removeResult.Succeeded)
                {
                    ModelState.AddModelError("", "No se pudieron eliminar los roles actuales.");
                    return View(model);
                }
            }

            // Agrega los nuevos
            if (selectedRoleNames.Any())
            {
                var addResult = await UserManager.AddToRolesAsync(user.Id, selectedRoleNames.ToArray());
                if (!addResult.Succeeded)
                {
                    ModelState.AddModelError("", "No se pudieron asignar los roles seleccionados.");
                    return View(model);
                }
            }

            TempData["Success"] = "Roles actualizados correctamente.";
            return RedirectToAction("Index");
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
