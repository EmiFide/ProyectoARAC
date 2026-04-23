using System.Collections.Generic;
using AdoptameLiberia.Models.Mascotas;

namespace AdoptameLiberia.Models.ViewModels
{
    public class CatalogoMascotasVM
    {
        public List<AnimalModel> Mascotas { get; set; } = new List<AnimalModel>();
        public HashSet<int> Favoritos { get; set; } = new HashSet<int>();
        public bool EsAdmin { get; set; }

        public string Tamano { get; set; }
        public decimal? PesoMinimo { get; set; }
        public decimal? PesoMaximo { get; set; }
        public int? EdadMinima { get; set; }
        public int? EdadMaxima { get; set; }
        public string Personalidad { get; set; }
    }
}