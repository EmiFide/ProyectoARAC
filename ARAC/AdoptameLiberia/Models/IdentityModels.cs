using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AdoptameLiberia.Models
{
    // Para agregar datos de perfil del usuario, agregue más propiedades a su clase ApplicationUser. Visite https://go.microsoft.com/fwlink/?LinkID=317594 para obtener más información.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, string> manager)
        {
            // Tenga en cuenta que authenticationType debe coincidir con el valor definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Agregar reclamaciones de usuario personalizadas aquí
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<Module> Modules { get; set; }
        public DbSet<RoleModulePermission> RoleModulePermissions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Clave compuesta (RoleId + ModuleId)
            modelBuilder.Entity<RoleModulePermission>()
                .HasKey(x => new { x.RoleId, x.ModuleId });

            modelBuilder.Entity<RoleModulePermission>()
                .HasRequired(x => x.Role)
                .WithMany(r => r.ModulePermissions)
                .HasForeignKey(x => x.RoleId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<RoleModulePermission>()
                .HasRequired(x => x.Module)
                .WithMany(m => m.RolePermissions)
                .HasForeignKey(x => x.ModuleId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Module>()
                .Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<AdoptameLiberia.Models.Raza> Razas { get; set; }

        public System.Data.Entity.DbSet<AdoptameLiberia.Models.TiposAnimales.TipoAnimal> TipoAnimals { get; set; }
    }
}