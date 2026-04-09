using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdoptameLiberia.Models.Finanzas
{
    [Table("CategoriaFinanciera")] // 🔥 IMPORTANTE
    public class CategoriaFinanciera
    {
        [Key]
        public int ID_Categoria { get; set; }

        public string Nombre { get; set; }

        public string Tipo { get; set; }

        public bool? Estado { get; set; }

        public DateTime? FechaRegistro { get; set; }
    }
}