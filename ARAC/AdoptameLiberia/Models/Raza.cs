using System.ComponentModel.DataAnnotations;

namespace AdoptameLiberia.Models
{
    public class Raza
    {
        [Key]
        public int RazaId { get; set; }

        public string NombreRaza { get; set; }

        public string Descripcion { get; set; } 

    }
}