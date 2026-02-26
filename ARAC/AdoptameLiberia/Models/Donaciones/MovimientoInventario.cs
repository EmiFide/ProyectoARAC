using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Donaciones
{
    [Table("Movimiento_Inventario")]
    public class MovimientoInventario
    {
        [Key]
        [Column("ID_Movimiento_Inventario")]
        public int IdMovimientoInventario { get; set; }

        [Column("ID_Item_Inventario")]
        public int IdItemInventario { get; set; }

        [Column("Tipo_Movimiento")]
        [StringLength(20)]
        public string TipoMovimiento { get; set; } // Entrada/Salida

        [Column("Fecha_Movimiento")]
        public DateTime FechaMovimiento { get; set; }

        [Column("Motivo")]
        [StringLength(200)]
        public string Motivo { get; set; }
    }
}