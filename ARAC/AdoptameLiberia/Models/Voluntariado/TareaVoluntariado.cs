using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Voluntariado
{
    [Table("Tarea_Voluntariado")]
    public class TareaVoluntariado
    {
        [Key]
        public int ID_Tarea { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        public TimeSpan? Hora { get; set; }

        public string Lugar { get; set; }

        public bool Estado { get; set; }

        public DateTime Fecha_Registro { get; set; }
    }
}