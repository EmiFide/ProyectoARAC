using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Educativo
{
    public class ContenidoFavorito
    {
        [Key]
        public int IdContenidoFavorito { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [Required]
        public int IdContenidoEducativo { get; set; }

        public DateTime FechaAgregado { get; set; }

        [ForeignKey("IdContenidoEducativo")]
        public virtual ContenidoEducativo ContenidoEducativo { get; set; }

        [ForeignKey("UsuarioId")]
        public virtual ApplicationUser Usuario { get; set; }
    }
}