using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AdoptameLiberia.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, string> manager)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            userIdentity.AddClaim(new Claim("Nombre", this.Nombre ?? string.Empty));
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserLogin, IdentityUserRole, IdentityUserClaim>
    {
        // 🔥 CONSTRUCTOR CORREGIDO (CLAVE)
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
            Database.SetInitializer<ApplicationDbContext>(null); // ❗ DESACTIVA VALIDACIÓN DE MODELO
        }

        // 🔹 EXISTENTE
        public DbSet<Module> Modules { get; set; }
        public DbSet<RoleModulePermission> RoleModulePermissions { get; set; }

        // 🔥 DONACIONES
        public DbSet<AdoptameLiberia.Models.Donaciones.Donacion> Donaciones { get; set; }
        public DbSet<AdoptameLiberia.Models.Donaciones.DetalleDonacion> DetalleDonaciones { get; set; }
        public DbSet<AdoptameLiberia.Models.Donaciones.ObservacionDonacion> ObservacionesDonacion { get; set; }

        // 🔥 FINANZAS
        public DbSet<AdoptameLiberia.Models.Finanzas.Gasto> Gastos { get; set; }
        public DbSet<AdoptameLiberia.Models.Finanzas.CategoriaFinanciera> CategoriasFinancieras { get; set; }

        // 🔥 NOTICIAS
        public DbSet<AdoptameLiberia.Models.Noticias.Noticia> Noticias { get; set; }

        // 🔹 EXISTENTE
        public DbSet<AdoptameLiberia.Models.Raza> Razas { get; set; }
        public DbSet<AdoptameLiberia.Models.TiposAnimales.TipoAnimal> TipoAnimals { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
    }
}