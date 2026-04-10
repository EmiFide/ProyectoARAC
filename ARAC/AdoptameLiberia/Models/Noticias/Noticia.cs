using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Noticias
{
    [Table("Noticia")] // 🔥 IMPORTANTE
    public class Noticia
    {
        [Key]
        public int ID_Noticia { get; set; }

        public string ID_Usuario { get; set; }

        [Required(ErrorMessage = "El título es obligatorio")]
        public string Titulo { get; set; }


        [Required(ErrorMessage = "El contenido es obligatorio")]
        public string Contenido { get; set; }

        public DateTime? Fecha_Publicacion { get; set; }

        public bool Estado { get; set; }

        public int Likes { get; set; } = 0;
    }
}