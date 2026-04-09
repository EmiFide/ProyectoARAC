using AdoptameLiberia.Models.TiposAnimales;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models
{
    public class Raza
    {
        [Key]
        public int ID_Raza { get; set; }

        [DisplayName("Nombre de la raza")]
        public string NombreRaza { get; set; }

        [DisplayName("Descripción")]
        public string Descripcion { get; set; }

        [ForeignKey("TipoAnimal")]
        [Required]
        public int ID_TipoAnimal { get; set; }

        public TipoAnimal TipoAnimal { get; set; }
    }
}