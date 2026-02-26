using AdoptameLiberia.Models.TiposAnimales;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models
{
    public class Raza
    {
        [Key]
        public int ID_Raza { get; set; }

        public string NombreRaza { get; set; }

        public string Descripcion { get; set; }

        [ForeignKey("TipoAnimal")]
        [Required]
        public int ID_TipoAnimal { get; set; }

        public TipoAnimal TipoAnimal { get; set; }
    }
}