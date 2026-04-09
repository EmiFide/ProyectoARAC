using System.Collections.Generic;

namespace AdoptameLiberia.Models.Finanzas
{
    public class ReporteFinancieroVM
    {
        public decimal TotalIngresos { get; set; }
        public decimal TotalGastos { get; set; }
        public decimal Balance { get; set; }
        public int TotalMovimientos { get; set; }

        public List<Gasto> Gastos { get; set; }
        public List<CategoriaFinanciera> Categorias { get; set; }

        public ReporteFinancieroVM()
        {
            Gastos = new List<Gasto>();
            Categorias = new List<CategoriaFinanciera>();
        }
    }
}