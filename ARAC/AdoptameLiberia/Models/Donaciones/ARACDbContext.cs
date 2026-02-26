using System.Data.Entity;

namespace AdoptameLiberia.Models.Donaciones
{
    public class ARACDbContext : DbContext
    {
        public ARACDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<Donacion> Donaciones { get; set; }
        public DbSet<TipoDonacion> TiposDonacion { get; set; }
        public DbSet<DetalleDonacion> DetallesDonacion { get; set; }
        public DbSet<ItemInventario> ItemsInventario { get; set; }
        public DbSet<MovimientoInventario> MovimientosInventario { get; set; }
        public DbSet<ObservacionDonacion> ObservacionesDonacion { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // IMPORTANTÍSIMO: evita que EF trate de crear/migrar tablas
            base.OnModelCreating(modelBuilder);
        }
    }
}