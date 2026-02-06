using System;
using System.Collections.Generic;
using System.Linq;
using AdoptameLiberia.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AdoptameLiberia.App_Start
{
    public static class SeedData
    {
        // Ejecuta seed seguro (idempotente): no duplica datos si ya existen.
        public static void EnsureSeed()
        {
            using (var db = new ApplicationDbContext())
            {
                // 1) Módulos base
                var defaultModules = new[]
                {
                    new Module { Name = "Animales", Description = "Gestión de animales rescatados" },
                    new Module { Name = "Adopciones", Description = "Gestión de procesos de adopción" },
                    new Module { Name = "Donaciones", Description = "Gestión de donaciones" },
                    new Module { Name = "Usuarios", Description = "Gestión de usuarios" },
                    new Module { Name = "Roles y permisos", Description = "Gestión de roles y permisos" },
                };

                if (!db.Modules.Any())
                {
                    db.Modules.AddRange(defaultModules);
                    db.SaveChanges();
                }
                else
                {
                    // Inserta módulos faltantes por nombre
                    foreach (var m in defaultModules)
                    {
                        if (!db.Modules.Any(x => x.Name == m.Name))
                        {
                            db.Modules.Add(m);
                        }
                    }
                    db.SaveChanges();
                }

                // 2) Roles base
                var roleStore = new RoleStore<ApplicationRole>(db);
                var roleManager = new RoleManager<ApplicationRole>(roleStore);

                EnsureRole(roleManager, "Administrador", "Acceso total al sistema");
                EnsureRole(roleManager, "Colaborador", "Acceso operativo (lectura/escritura según permisos)");
                EnsureRole(roleManager, "Lector", "Acceso solo lectura");

                // 3) Permisos por defecto
                // Administrador: escritura en todo
                var admin = roleManager.FindByName("Administrador");
                if (admin != null)
                {
                    var modules = db.Modules.ToList();
                    foreach (var mod in modules)
                    {
                        var perm = db.RoleModulePermissions.FirstOrDefault(p => p.RoleId == admin.Id && p.ModuleId == mod.ModuleId);
                        if (perm == null)
                        {
                            db.RoleModulePermissions.Add(new RoleModulePermission
                            {
                                RoleId = admin.Id,
                                ModuleId = mod.ModuleId,
                                CanRead = true,
                                CanWrite = true
                            });
                        }
                        else
                        {
                            perm.CanRead = true;
                            perm.CanWrite = true;
                        }
                    }
                    db.SaveChanges();
                }

                // Lector: lectura en todo
                var reader = roleManager.FindByName("Lector");
                if (reader != null)
                {
                    var modules = db.Modules.ToList();
                    foreach (var mod in modules)
                    {
                        var perm = db.RoleModulePermissions.FirstOrDefault(p => p.RoleId == reader.Id && p.ModuleId == mod.ModuleId);
                        if (perm == null)
                        {
                            db.RoleModulePermissions.Add(new RoleModulePermission
                            {
                                RoleId = reader.Id,
                                ModuleId = mod.ModuleId,
                                CanRead = true,
                                CanWrite = false
                            });
                        }
                    }
                    db.SaveChanges();
                }
            }
        }

        private static void EnsureRole(RoleManager<ApplicationRole> roleManager, string roleName, string description)
        {
            if (!roleManager.RoleExists(roleName))
            {
                roleManager.Create(new ApplicationRole { Name = roleName, Description = description });
            }
        }
    }
}
