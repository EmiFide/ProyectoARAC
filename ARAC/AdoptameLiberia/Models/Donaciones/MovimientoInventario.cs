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

        [Column("Cantidad")]
        [Range(0, int.MaxValue)]
        public int Cantidad { get; set; }

        [Column("Stock_Anterior")]
        public int? StockAnterior { get; set; }

        [Column("Stock_Nuevo")]
        public int? StockNuevo { get; set; }

        [Column("Destinatario")]
        [StringLength(100)]
        public string Destinatario { get; set; }

        [Column("Fecha_Movimiento")]
        public DateTime FechaMovimiento { get; set; }

        [Column("UsuarioId")]
        [StringLength(128)]
        public string UsuarioId { get; set; }

        [Column("Motivo")]
        [StringLength(200)]
        public string Motivo { get; set; }
    }
}