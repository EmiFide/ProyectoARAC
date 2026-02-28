using System.ComponentModel.DataAnnotations;

namespace AdoptameLiberia.Models.Inventario
{
    public class AjusteInventario
    {
        public int IdItemInventario { get; set; }
        public string NombreItem { get; set; }

        public int StockActual { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "El nuevo stock no puede ser negativo.")]
        public int NuevoStock { get; set; }

        [Required(ErrorMessage = "Debes indicar el motivo del ajuste.")]
        [StringLength(200)]
        public string Motivo { get; set; }
    }
}
