using System.ComponentModel.DataAnnotations;

namespace AdoptameLiberia.Models.Inventario
{
    public class MovimientoSalida
    {
        public int IdItemInventario { get; set; }
        public string NombreItem { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El destinatario/uso es obligatorio.")]
        [StringLength(100)]
        public string Destinatario { get; set; }

        [StringLength(200)]
        public string Motivo { get; set; }
    }
}
