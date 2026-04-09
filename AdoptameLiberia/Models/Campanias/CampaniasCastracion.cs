using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AdoptameLiberia.Models.Campanias
{
    [Table("CampaniasCastracion")]

    public class CampaniaCastracion
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Nombre de la campania")]
        public string Nombre { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [Required]
        public string Lugar { get; set; }

        [Required]
        [Display(Name = "Cupos disponibles")]
        public int Cupos { get; set; }

        public virtual ICollection<InscripcionCastracion> Inscripciones { get; set; }
    }
}