using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AdoptameLiberia.Models
{
    public class Raza
    {
        [Key]
        public int ID_Raza { get; set; }

        [Required(ErrorMessage = "El nombre de la raza es obligatorio.")]
        [StringLength(100)]
        [DisplayName("Nombre de la raza")]
        public string Nombre { get; set; }

        [StringLength(255)]
        [DisplayName("Descripción")]
        public string Descripcion { get; set; }
    }
}