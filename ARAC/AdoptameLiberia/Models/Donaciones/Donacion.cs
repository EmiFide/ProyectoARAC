using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Donaciones
{
    [Table("Donacion")]
    public class Donacion
    {
        [Key]
        [Column("ID_Donacion")]
        public int IdDonacion { get; set; }

        [Column("ID_Usuario")]
        public int IdUsuario { get; set; }

        [Column("ID_Tipo_Donacion")]
        public int IdTipoDonacion { get; set; }

        [Column("Monto")]
        public decimal Monto { get; set; } // en tu BD no es nullable

        [Column("Fecha")]
        public DateTime Fecha { get; set; }

        [Column("Metodo")]
        [StringLength(50)]
        public string Metodo { get; set; }

        [Column("Descripcion")]
        [StringLength(200)]
        public string Descripcion { get; set; }

        [Column("Fecha_Registro")]
        public DateTime FechaRegistro { get; set; }

        // Navegación
        [ForeignKey(nameof(IdTipoDonacion))]
        public virtual TipoDonacion TipoDonacion { get; set; }

        public virtual ICollection<DetalleDonacion> Detalles { get; set; }
        public virtual ICollection<ObservacionDonacion> Observaciones { get; set; }
    }
}