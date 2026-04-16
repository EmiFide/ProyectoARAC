using System.Web;

namespace AdoptameLiberia.Models.ViewModel
{
    public class RegistrarAnimalVM
    {
        public string Nombre_Animal { get; set; }

        public int ID_Raza { get; set; }

        public int ID_TipoAnimal { get; set; }

        public int? Edad { get; set; }

        public string Sexo { get; set; }

        public string Tamano { get; set; }

        public decimal? Peso { get; set; }

        public string Descripcion { get; set; }

        // 🔥 Esto es lo nuevo
        public HttpPostedFileBase ImagenArchivo { get; set; }
    }
}