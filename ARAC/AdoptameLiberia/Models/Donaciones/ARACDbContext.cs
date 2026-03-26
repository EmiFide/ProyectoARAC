using System.Data.Entity;
using AdoptameLiberia.Models.Campanias;
using AdoptameLiberia.Models.Mascotas;
using AdoptameLiberia.Models.Voluntariado;
using AdoptameLiberia.Models.Comunidad;

namespace AdoptameLiberia.Models.Donaciones
{
    public class ARACDbContext : DbContext
    {
        public ARACDbContext() : base("DefaultConnection")
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Donacion> Donaciones { get; set; }
        public DbSet<TipoDonacion> TiposDonacion { get; set; }
        public DbSet<DetalleDonacion> DetallesDonacion { get; set; }
        public DbSet<ItemInventario> ItemsInventario { get; set; }
        public DbSet<MovimientoInventario> MovimientosInventario { get; set; }
        public DbSet<ObservacionDonacion> ObservacionesDonacion { get; set; }
        public DbSet<CampaniaCastracion> CampaniasCastracion { get; set; }
        public DbSet<InscripcionCastracion> InscripcionesCastracion { get; set; }
        public DbSet<Voluntario> Voluntarios { get; set; }
        public DbSet<TareaVoluntariado> TareasVoluntariado { get; set; }
        public DbSet<ParticipacionVoluntario> ParticipacionesVoluntario { get; set; }
        public DbSet<PublicacionComunidad> PublicacionesComunidad { get; set; }
        public DbSet<ComentarioPublicacion> ComentariosPublicacion { get; set; }
        public DbSet<AnimalModel> Animales { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // IMPORTANTÍSIMO: evita que EF trate de crear/migrar tablas
            base.OnModelCreating(modelBuilder);
        }
    }
}