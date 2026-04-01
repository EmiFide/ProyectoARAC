using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Finanzas
{
    [Table("Gasto")] // 🔥 IMPORTANTE
    public class Gasto
    {
        [Key]
        public int ID_Gasto { get; set; }

        public int? ID_Categoria { get; set; }

        public decimal? Monto { get; set; }

        public string Descripcion { get; set; }

        public DateTime? Fecha { get; set; }

        [ForeignKey("ID_Categoria")]
        public virtual CategoriaFinanciera Categoria { get; set; }
    }
}