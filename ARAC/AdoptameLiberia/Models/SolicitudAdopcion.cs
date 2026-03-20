using AdoptameLiberia.Models.Mascotas;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models
{
    public class SolicitudAdopcion
    {
        [Key]
        public int ID_Solicitud { get; set; }

        [NotMapped]
        public int ID_Usuario { get; set; }
        public int ID_Animal { get; set; }

        public string Condiciones_Hogar { get; set; }
        public string Motivo_Adopcion { get; set; }
        public bool Otros_Animales { get; set; }
        public string Detalle_Otros_Animales { get; set; }

        public DateTime Fecha_Solicitud { get; set; }

        public string Estado { get; set; }

        public virtual AspNetUsers Usuario { get; set; }
        public virtual AnimalModel Animal { get; set; }
    }
}