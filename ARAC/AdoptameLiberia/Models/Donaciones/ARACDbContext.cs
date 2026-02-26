using System.Data.Entity;
using AdoptameLiberia.Models.Campanias;
using AdoptameLiberia.Models.Mascotas;

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
        public DbSet<CampaniaCastracion> CampaniasCastracion { get; set; }
        public DbSet<InscripcionCastracion> InscripcionesCastracion { get; set; }
        public DbSet<AnimalModel> Animal { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // IMPORTANTÍSIMO: evita que EF trate de crear/migrar tablas
            base.OnModelCreating(modelBuilder);
        }
    }
}