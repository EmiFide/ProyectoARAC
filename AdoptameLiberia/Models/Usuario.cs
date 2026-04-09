using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        public int ID_Usuario { get; set; }

        public int ID_Rol { get; set; }

        public string Nombre { get; set; }

        public string Apellido1 { get; set; }

        public string Apellido2 { get; set; }

        public string Correo { get; set; }

        public string IdAspNetUser { get; set; }
    }
}