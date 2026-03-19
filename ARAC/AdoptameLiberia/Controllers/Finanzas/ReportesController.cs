using System.Linq;
using System.Web.Mvc;
using AdoptameLiberia.Models;
using AdoptameLiberia.Models.ViewModels;

namespace AdoptameLiberia.Controllers.Finanzas
{
    public class ReportesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Dashboard()
        {
            var vm = new ReporteFinancieroVM
            {
                TotalIngresos = db.Donaciones.Sum(d => (decimal?)d.Monto) ?? 0,
                TotalGastos = db.Gastos.Sum(g => (decimal?)g.Monto) ?? 0
            };

            vm.Balance = vm.TotalIngresos - vm.TotalGastos;

            return View(vm);
        }
    }
}