using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdoptameLiberia.Models
{
    // ---------- Roles ----------
    public class RolePermissionItemVM
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; }

        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
    }

    public class RoleCreateEditVM
    {
        public string Id { get; set; }

        [Required, StringLength(256)]
        [Display(Name = "Nombre del rol")]
        public string Name { get; set; }

        [StringLength(256)]
        [Display(Name = "Descripción")]
        public string Description { get; set; }

        public List<RolePermissionItemVM> Permissions { get; set; } = new List<RolePermissionItemVM>();
    }

    public class RoleListItemVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        // Para mostrar en Index (tabla)
        public List<RolePermissionItemVM> Permissions { get; set; } = new List<RolePermissionItemVM>();
    }

    // ---------- Usuarios ----------
    public class UserListItemVM
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class UserAssignRolesVM
    {
        public string UserId { get; set; }
        public string Email { get; set; }

        // Roles disponibles
        public List<RoleCheckboxVM> AvailableRoles { get; set; } = new List<RoleCheckboxVM>();
    }

    public class RoleCheckboxVM
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool Selected { get; set; }
    }
}
