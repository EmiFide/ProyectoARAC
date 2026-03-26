using AdoptameLiberia.Models.Mascotas;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Campanias
{
    [Table("InscripcionesCastracion")]
    public class InscripcionCastracion
    {
        [Key]
        public int Id { get; set; }

        public int CampaniaCastracionId { get; set; }

        public int AnimalId { get; set; }

        public int? IdUsuario { get; set; }

        public string VeterinarioAsignado { get; set; }

        public string Resultado { get; set; }

        [ForeignKey("AnimalId")]
        public virtual AnimalModel Animal { get; set; }

        [ForeignKey("CampaniaCastracionId")]
        public virtual CampaniaCastracion Campania { get; set; }
    }
}