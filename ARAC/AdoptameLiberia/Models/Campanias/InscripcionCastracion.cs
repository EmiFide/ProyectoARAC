using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AdoptameLiberia.Models.Mascotas;

namespace AdoptameLiberia.Models.Campanias
{
    [Table("InscripcionesCastracion")]
    public class InscripcionCastracion
    {
        public int Id { get; set; }

        [Required]
        public int CampaniaCastracionId { get; set; }

        [ForeignKey("CampaniaCastracionId")]
        public virtual CampaniaCastracion Campania { get; set; }

        [Required]
        public int AnimalId { get; set; }

        [ForeignKey("AnimalId")]
        public virtual AnimalModel Animal { get; set; }

        public string VeterinarioAsignado { get; set; }

        public string Resultado { get; set; }
    }
}