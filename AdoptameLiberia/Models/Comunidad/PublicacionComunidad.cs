using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Comunidad
{
    [Table("Publicacion_Comunidad")]
    public class PublicacionComunidad
    {
        [Key]
        public int ID_Publicacion { get; set; }

        public int ID_Usuario { get; set; }

        public int? ID_Categoria { get; set; }

        [Required]
        public string Titulo { get; set; }

        [Required]
        public string Contenido { get; set; }

        public DateTime Fecha { get; set; }

        public bool Estado { get; set; }
    }
}