using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AdoptameLiberia.Models
{
    // Rol extendido para poder guardar descripción y relacionar permisos por módulo.
    public class ApplicationRole : IdentityRole
    {
        [StringLength(256)]
        public string Description { get; set; }

        public virtual ICollection<RoleModulePermission> ModulePermissions { get; set; } = new HashSet<RoleModulePermission>();
    }

    // Catálogo de módulos del sistema (por ejemplo: Animales, Adopciones, Donaciones, Usuarios, Roles y permisos, etc.)
    public class Module
    {
        [Key]
        public int ModuleId { get; set; }

        [Required, StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        public virtual ICollection<RoleModulePermission> RolePermissions { get; set; } = new HashSet<RoleModulePermission>();
    }

    // Permisos por rol y por módulo.
    // CanWrite implica CanRead (lo reforzamos en la lógica de guardado y en consultas).
    public class RoleModulePermission
    {
        [Key, Column(Order = 0)]
        [Required]
        public string RoleId { get; set; }

        [Key, Column(Order = 1)]
        [Required]
        public int ModuleId { get; set; }

        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual ApplicationRole Role { get; set; }

        [ForeignKey(nameof(ModuleId))]
        public virtual Module Module { get; set; }
    }
}
