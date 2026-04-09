using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models
{
    [Table("Solicitud_Adopcion")]
    public class SolicitudAdopcion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID_Solicitud")]
        public int ID_Solicitud { get; set; }

        [Column("ID_Usuario")]
        public int? ID_Usuario { get; set; }

        [Column("ID_Animal")]
        public int? ID_Animal { get; set; }

        [Column("Condiciones_Hogar")]
        [StringLength(300)]
        public string Condiciones_Hogar { get; set; }

        [Column("Motivo_Adopcion")]
        [StringLength(200)]
        public string Motivo_Adopcion { get; set; }

        [Column("Otros_Animales")]
        public bool? Otros_Animales { get; set; }

        [Column("Fecha_Solicitud")]
        public DateTime? Fecha_Solicitud { get; set; }

        [Column("Detalle_Otros_Animales")]
        [StringLength(200)]
        public string Detalle_Otros_Animales { get; set; }

        [Column("Estado")]
        [StringLength(50)]
        public string Estado { get; set; }
    }
}