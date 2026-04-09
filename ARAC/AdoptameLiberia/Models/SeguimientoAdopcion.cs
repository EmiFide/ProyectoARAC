using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models
{
    [Table("Seguimiento_Adopcion")]
    public class SeguimientoAdopcion
    {
        [Key]
        [Column("ID_Seguimiento")]
        public int ID_Seguimiento { get; set; }

        [Required]
        [Column("ID_Adopcion")]
        public int ID_Adopcion { get; set; }

        [Column("ID_Usuario")]
        public int? ID_Usuario { get; set; }

        [Column("Fecha_Seguimiento")]
        public DateTime Fecha_Seguimiento { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Tipo_Seguimiento")]
        [Display(Name = "Tipo de seguimiento")]
        public string Tipo_Seguimiento { get; set; }

        [StringLength(100)]
        [Column("Estado_Mascota")]
        [Display(Name = "Estado de la mascota")]
        public string Estado_Mascota { get; set; }

        [StringLength(100)]
        [Column("Estado_Hogar")]
        [Display(Name = "Estado del hogar")]
        public string Estado_Hogar { get; set; }

        [Required]
        [StringLength(500)]
        public string Comentario { get; set; }

        [StringLength(200)]
        [Column("Proxima_Accion")]
        [Display(Name = "Próxima acción")]
        public string Proxima_Accion { get; set; }
    }
}