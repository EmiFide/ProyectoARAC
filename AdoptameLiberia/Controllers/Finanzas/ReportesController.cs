using System.Linq;
using System.Web.Mvc;
using AdoptameLiberia.Models;
using AdoptameLiberia.Models.Finanzas;
using System.Data.Entity;

namespace AdoptameLiberia.Controllers.Finanzas
{
    public class ReportesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Dashboard()
        {
            var gastos = db.Gastos
                           .Include(g => g.Categoria)
                           .ToList();

            var vm = new ReporteFinancieroVM
            {
                Gastos = gastos,
                TotalIngresos = db.Donaciones.Sum(d => (decimal?)d.Monto) ?? 0,
                TotalGastos = gastos.Sum(g => (decimal?)g.Monto) ?? 0,
                TotalMovimientos = gastos.Count()
            };

            vm.Balance = vm.TotalIngresos - vm.TotalGastos;

            return View("~/Views/Reporte/Dashboard.cshtml", vm);
        }
    }
}