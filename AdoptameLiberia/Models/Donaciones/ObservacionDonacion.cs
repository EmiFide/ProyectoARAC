using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Donaciones
{
    [Table("Observacion_Donacion")]
    public class ObservacionDonacion
    {
        [Key]
        [Column("ID_Observacion")]
        public int IdObservacion { get; set; }

        [Column("ID_Donacion")]
        public int IdDonacion { get; set; }

        [Required]
        [Column("Comentario")]
        [StringLength(400)]
        public string Comentario { get; set; }

        [Column("Fecha")]
        public DateTime Fecha { get; set; }

        [ForeignKey(nameof(IdDonacion))]
        public virtual Donacion Donacion { get; set; }
    }
}