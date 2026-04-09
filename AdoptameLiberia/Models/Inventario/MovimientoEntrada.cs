using System.ComponentModel.DataAnnotations;

namespace AdoptameLiberia.Models.Inventario
{
    public class MovimientoEntrada
    {
        public int IdItemInventario { get; set; }
        public string NombreItem { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }

        [StringLength(200)]
        public string Motivo { get; set; }
    }
}
