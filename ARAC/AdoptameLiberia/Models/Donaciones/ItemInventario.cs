using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Donaciones
{
    [Table("Item_Inventario")]
    public class ItemInventario
    {
        [Key]
        [Column("ID_Item_Inventario")]
        public int IdItemInventario { get; set; }

        [Column("Nombre")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Column("Descripcion")]
        [StringLength(200)]
        public string Descripcion { get; set; }

        [Column("Categoria")]
        [StringLength(50)]
        public string Categoria { get; set; }

        [Column("Unidad_Medida")]
        [StringLength(20)]
        public string UnidadMedida { get; set; }

        [Column("Stock_Actual")]
        public int StockActual { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }

        [Column("Fecha_Registro")]
        public DateTime FechaRegistro { get; set; }
    }
}