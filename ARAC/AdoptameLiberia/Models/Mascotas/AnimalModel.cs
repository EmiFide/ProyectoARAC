using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdoptameLiberia.Models.Mascotas
{
    public class AnimalModel
    {
        public int ID_Animal { get; set; }
        public string Nombre_Animal { get; set; }
        public int ID_Raza { get; set; }
        public int ID_TipoAnimal { get; set; }
        public int? Edad { get; set; }
        public string Sexo { get; set; }
        public string Tamano { get; set; }
        public decimal? Peso { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public string NombreRaza { get; set; }
        public string NombreTipo { get; set; }
    }
}
