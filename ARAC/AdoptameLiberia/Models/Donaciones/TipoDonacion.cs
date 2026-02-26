using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Donaciones
{
    [Table("Tipo_Donacion")]
    public class TipoDonacion
    {
        [Key]
        [Column("ID_Tipo_Donacion")]
        public int IdTipoDonacion { get; set; }

        [Column("Nombre")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Column("Descripcion")]
        [StringLength(200)]
        public string Descripcion { get; set; }

        [Column("Fecha_Registro")]
        public DateTime FechaRegistro { get; set; }
    }
}