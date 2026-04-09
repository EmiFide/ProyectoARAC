using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Voluntariado
{
    [Table("Participacion_Voluntario")]
    public class ParticipacionVoluntario
    {
        [Key]
        public int ID_Participacion { get; set; }

        [Required]
        public int ID_Voluntario { get; set; }

        [Required]
        public int ID_Tarea { get; set; }

        public bool Asistio { get; set; }

        public string Observaciones { get; set; }

        public DateTime Fecha_Registro { get; set; }

        [ForeignKey("ID_Voluntario")]
        public virtual Voluntario Voluntario { get; set; }

        [ForeignKey("ID_Tarea")]
        public virtual TareaVoluntariado Tarea { get; set; }
    }
}