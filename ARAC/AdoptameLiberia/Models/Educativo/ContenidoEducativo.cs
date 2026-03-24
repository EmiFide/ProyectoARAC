using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdoptameLiberia.Models.Educativo
{
    public class ContenidoEducativo
    {
        [Key]
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
        [StringLength(50)]
        [Display(Name = "Tipo de contenido")]
        public string TipoContenido { get; set; }

        [Required]
        [StringLength(100)]
        public string Tema { get; set; }

        [StringLength(500)]
        [Display(Name = "URL del contenido")]
        public string UrlContenido { get; set; }

        [StringLength(500)]
        [Display(Name = "Ruta del archivo")]
        public string RutaArchivo { get; set; }

        public bool Activo { get; set; }

        [Display(Name = "Fecha de creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Fecha de actualización")]
        public DateTime? FechaActualizacion { get; set; }

        public string UsuarioCreadorId { get; set; }

        public virtual ICollection<ContenidoFavorito> Favoritos { get; set; }
    }
}