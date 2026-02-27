using System;
using System.ComponentModel.DataAnnotations;

namespace AdoptameLiberia.Models.Inventario
{
    public class ItemInventarioCreateEdit
    {
        public int IdItemInventario { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria.")]
        [StringLength(50)]
        public string Categoria { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        [StringLength(20)]
        public string UnidadMedida { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
        public int StockActual { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
        public int StockMinimo { get; set; }

        public DateTime? FechaCaducidad { get; set; }
    }
}
