using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Voluntariado
{
    [Table("Voluntario")]
    public class Voluntario
    {
        [Key]
        public int ID_Voluntario { get; set; }

        public int? ID_Usuario { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Apellido 1")]
        public string Apellido1 { get; set; }

        [Display(Name = "Apellido 2")]
        public string Apellido2 { get; set; }

        [Required]
        [Display(Name = "Correo")]
        public string Correo { get; set; }

        [Required]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; }

        [Display(Name = "Disponibilidad")]
        public string Disponibilidad { get; set; }

        [Display(Name = "Habilidades")]
        public string Habilidades { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        public DateTime Fecha_Registro { get; set; }
    }
}