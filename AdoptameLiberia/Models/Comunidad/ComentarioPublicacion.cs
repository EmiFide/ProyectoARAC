using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Comunidad
{
    [Table("Comentario_Publicacion")]
    public class ComentarioPublicacion
    {
        [Key]
        public int ID_Comentario { get; set; }

        [Required]
        public int ID_Publicacion { get; set; }

        [Required]
        public int ID_Usuario { get; set; }

        [Required]
        public string Contenido { get; set; }

        public DateTime Fecha { get; set; }

        public bool Estado { get; set; }

        [ForeignKey("ID_Publicacion")]
        public virtual PublicacionComunidad Publicacion { get; set; }
    }
}