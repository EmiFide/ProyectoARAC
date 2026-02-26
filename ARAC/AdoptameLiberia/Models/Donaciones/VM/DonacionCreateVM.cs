using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AdoptameLiberia.Models.Donaciones.VM
{
    public class DonacionCreateVM
    {
        [Required]
        public int IdTipoDonacion { get; set; } // 1=Monetaria, 2=Insumos

        [Required]
        public int IdUsuario { get; set; } // por ahora admin lo selecciona (o lo seteas desde Identity)

        [Required]
        [StringLength(50)]
        public string Metodo { get; set; }

        [StringLength(200)]
        public string Descripcion { get; set; }

        // Monetaria
        public decimal? Monto { get; set; }

        // Insumos
        public List<DetalleInsumoVM> Insumos { get; set; } = new List<DetalleInsumoVM>();

        // Dropdowns
        public IEnumerable<SelectListItem> TiposDonacion { get; set; }
        public IEnumerable<SelectListItem> ItemsInventario { get; set; }
    }

    public class DetalleInsumoVM
    {
        public int? IdItemInventario { get; set; }
        [StringLength(200)]
        public string Descripcion { get; set; } // Ej: "5 sacos de alimento"
    }
}