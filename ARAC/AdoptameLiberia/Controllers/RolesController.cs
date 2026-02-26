using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AdoptameLiberia.Models;
using Microsoft.AspNet.Identity.Owin;

namespace AdoptameLiberia.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class RolesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        private ApplicationRoleManager RoleManager => HttpContext.GetOwinContext().Get<ApplicationRoleManager>();

        // HU-2.3: visualizar + buscar
        public ActionResult Index(string q = null)
        {
            var rolesQuery = db.Roles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                rolesQuery = rolesQuery.Where(r => r.Name.Contains(q));
            }

            var roles = rolesQuery
                .OrderBy(r => r.Name)
                .ToList();

            var modules = db.Modules.OrderBy(m => m.Name).ToList();

            var vm = roles.Select(r => new RoleListItemVM
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Permissions = modules.Select(m =>
                {
                    var perm = db.RoleModulePermissions.FirstOrDefault(p => p.RoleId == r.Id && p.ModuleId == m.ModuleId);
                    return new RolePermissionItemVM
                    {
                        ModuleId = m.ModuleId,
                        ModuleName = m.Name,
                        CanRead = perm?.CanRead ?? false,
                        CanWrite = perm?.CanWrite ?? false
                    };
                }).ToList()
            }).ToList();

            ViewBag.Query = q;
            return View(vm);
        }

        // HU-2.2: crear
        public ActionResult Create()
        {
            var modules = db.Modules.OrderBy(m => m.Name).ToList();

            var vm = new RoleCreateEditVM
            {
                Permissions = modules.Select(m => new RolePermissionItemVM
                {
                    ModuleId = m.ModuleId,
                    ModuleName = m.Name,
                    CanRead = false,
                    CanWrite = false
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RoleCreateEditVM model)
        {
            if (!ModelState.IsValid)
            {
                // recarga nombres de módulos por seguridad
                HydrateModuleNames(model);
                return View(model);
            }

            // Validación: nombre único
            if (db.Roles.Any(r => r.Name == model.Name))
            {
                ModelState.AddModelError("Name", "Ya existe un rol con ese nombre.");
                HydrateModuleNames(model);
                return View(model);
            }

            var role = new ApplicationRole
            {
                Name = model.Name.Trim(),
                Description = model.Description
            };

            var result = await RoleManager.CreateAsync(role);
            if (!result.Succeeded)
            {
                AddIdentityErrors(result.Errors);
                HydrateModuleNames(model);
                return View(model);
            }

            SavePermissions(role.Id, model.Permissions);

            TempData["Success"] = "Rol creado correctamente.";
            return RedirectToAction("Index");
        }

        // HU-2.4: editar
        public ActionResult Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return HttpNotFound();

            var role = db.Roles.FirstOrDefault(r => r.Id == id);
            if (role == null) return HttpNotFound();

            var modules = db.Modules.OrderBy(m => m.Name).ToList();

            var vm = new RoleCreateEditVM
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                Permissions = modules.Select(m =>
                {
                    var perm = db.RoleModulePermissions.FirstOrDefault(p => p.RoleId == role.Id && p.ModuleId == m.ModuleId);
                    return new RolePermissionItemVM
                    {
                        ModuleId = m.ModuleId,
                        ModuleName = m.Name,
                        CanRead = perm?.CanRead ?? false,
                        CanWrite = perm?.CanWrite ?? false
                    };
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(RoleCreateEditVM model)
        {
            if (!ModelState.IsValid)
            {
                HydrateModuleNames(model);
                return View(model);
            }

            var role = await RoleManager.FindByIdAsync(model.Id);
            if (role == null) return HttpNotFound();

            // Validación: nombre no duplicado (excluyendo el rol actual)
            var existing = await RoleManager.FindByNameAsync(model.Name);
            if (existing != null && existing.Id != model.Id)
            {
                ModelState.AddModelError("Name", "Ya existe un rol con ese nombre.");
                HydrateModuleNames(model);
                return View(model);
            }

            role.Name = model.Name.Trim();
            role.Description = model.Description;

            var result = await RoleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                AddIdentityErrors(result.Errors);
                HydrateModuleNames(model);
                return View(model);
            }

            SavePermissions(role.Id, model.Permissions);

            TempData["Success"] = "Rol actualizado correctamente.";
            return RedirectToAction("Index");
        }

        // ---------- Helpers ----------
        private void SavePermissions(string roleId, List<RolePermissionItemVM> permissions)
        {
            // Regla: CanWrite implica CanRead
            foreach (var p in permissions)
            {
                if (p.CanWrite) p.CanRead = true;
            }

            // Elimina permisos existentes y vuelve a crear (simple y seguro)
            var existing = db.RoleModulePermissions.Where(x => x.RoleId == roleId).ToList();
            if (existing.Any())
            {
                db.RoleModulePermissions.RemoveRange(existing);
                db.SaveChanges();
            }

            foreach (var p in permissions)
            {
                // Si no tiene lectura ni escritura, no se guarda fila (evita basura)
                if (!p.CanRead && !p.CanWrite) continue;

                db.RoleModulePermissions.Add(new RoleModulePermission
                {
                    RoleId = roleId,
                    ModuleId = p.ModuleId,
                    CanRead = p.CanRead,
                    CanWrite = p.CanWrite
                });
            }

            db.SaveChanges();
        }

        private void HydrateModuleNames(RoleCreateEditVM model)
        {
            var modules = db.Modules.ToDictionary(m => m.ModuleId, m => m.Name);
            foreach (var p in model.Permissions)
            {
                if (modules.ContainsKey(p.ModuleId))
                {
                    p.ModuleName = modules[p.ModuleId];
                }
            }
        }

        private void AddIdentityErrors(IEnumerable<string> errors)
        {
            foreach (var e in errors)
            {
                ModelState.AddModelError("", e);
            }
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
