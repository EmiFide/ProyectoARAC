using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Donaciones
{
    [Table("Detalle_Donacion")]
    public class DetalleDonacion
    {
        [Key]
        [Column("ID_Det_Donacion")]
        public int IdDetalleDonacion { get; set; }

        [Column("ID_Donacion")]
        public int IdDonacion { get; set; }

        [Column("ID_Item_Inventario")]
        public int? IdItemInventario { get; set; }

        [Column("Descripcion")]
        [StringLength(200)]
        public string Descripcion { get; set; }

        [ForeignKey(nameof(IdDonacion))]
        public virtual Donacion Donacion { get; set; }
    }
}