using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace AdoptameLiberia.Models.Educativo.VM
{
    public class ContenidoEducativoForm
    {
        public int IdContenidoEducativo { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Título")]
        public string Titulo { get; set; }

        [Required]
        [StringLength(1000)]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required]
        [Display(Name = "Tipo de contenido")]
        public string TipoContenido { get; set; }

        [Required]
        public string Tema { get; set; }

        [Display(Name = "URL del contenido")]
        public string UrlContenido { get; set; }

        [Display(Name = "Archivo")]
        public HttpPostedFileBase Archivo { get; set; }

        public bool Activo { get; set; }

        public IEnumerable<SelectListItem> Tipos { get; set; }
        public IEnumerable<SelectListItem> Temas { get; set; }

        public string RutaArchivoActual { get; set; }
    }
}