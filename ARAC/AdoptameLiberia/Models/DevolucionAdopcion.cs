using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models
{
    [Table("Devolucion_Adopcion")]
    public class DevolucionAdopcion
    {
        [Key]
        [Column("ID_Devolucion")]
        public int ID_Devolucion { get; set; }

        [Required]
        [Column("ID_Adopcion")]
        public int ID_Adopcion { get; set; }

        [Column("ID_Usuario_Registro")]
        public int? ID_Usuario_Registro { get; set; }

        [Column("Fecha_Devolucion")]
        [Display(Name = "Fecha de devolución")]
        public DateTime Fecha_Devolucion { get; set; }

        [Required]
        [StringLength(500)]
        public string Motivo { get; set; }

        [StringLength(500)]
        public string Observacion { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Estado_Final_Animal")]
        [Display(Name = "Estado final del animal")]
        public string Estado_Final_Animal { get; set; }
    }
}