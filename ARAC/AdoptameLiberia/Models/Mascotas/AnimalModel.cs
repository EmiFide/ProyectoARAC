using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Mascotas
{
    [Table("Animal")]
    public class AnimalModel
    {
        [Key]
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

        [NotMapped]
        public string NombreRaza { get; set; }

        [NotMapped]
        public string NombreTipo { get; set; }

        [NotMapped]
        public bool EstaAdoptado { get; set; }
        public string UsuarioRegistroId { get; set; }
    }
}