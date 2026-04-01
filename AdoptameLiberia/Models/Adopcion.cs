using AdoptameLiberia.Models.Mascotas;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models
{
    [Table("Adopcion")]
    public class Adopcion
    {
        [Key]
        public int ID_Adopcion { get; set; }

        public int ID_Solicitud { get; set; }
        public int ID_Animal { get; set; }

        public DateTime Fecha_Adopcion { get; set; }
        public string Estado_Adopcion { get; set; }

        public string Seguimiento_Inicial { get; set; }

        public virtual SolicitudAdopcion Solicitud { get; set; }
        public virtual AnimalModel Animal { get; set; }
    }
}