using System.Collections.Generic;
using System.Web.Mvc;
using AdoptameLiberia.Models.Educativo;

namespace AdoptameLiberia.Models.Educativo.VM
{
    public class EducativoFiltro
    {
        public string TipoContenido { get; set; }
        public string Tema { get; set; }
        public string TextoBusqueda { get; set; }

        public List<ContenidoEducativo> Resultados { get; set; }

        public IEnumerable<SelectListItem> Tipos { get; set; }
        public IEnumerable<SelectListItem> Temas { get; set; }
    }
}