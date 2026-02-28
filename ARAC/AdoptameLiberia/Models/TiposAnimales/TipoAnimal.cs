using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.TiposAnimales
{
    [Table("Tipo_Animal")]
    public class TipoAnimal
    {
        [Key]
        public int ID_TipoAnimal { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre_Tipo_Animal { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        public bool Estado { get; set; }

        public ICollection<Raza> Razas { get; set; }
    }
}
