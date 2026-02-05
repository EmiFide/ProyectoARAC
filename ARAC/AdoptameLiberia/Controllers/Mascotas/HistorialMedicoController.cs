using AdoptameLiberia.Models.Mascotas;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace AdoptameLiberia.Controllers.Mascotas
{
    [Authorize]
    public class HistorialMedicoController : Controller
    {
        string conexion = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // H16 - Listar historial
        public ActionResult Index(int idAnimal)
        {
            List<HistorialMedicoModel> lista = new List<HistorialMedicoModel>();

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand(
                    "SELECT * FROM Historial_Medico WHERE ID_Animal=@id ORDER BY Fecha DESC", cn);
                cmd.Parameters.AddWithValue("@id", idAnimal);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    lista.Add(new HistorialMedicoModel
                    {
                        ID_Historial = (int)dr["ID_Historial"],
                        ID_Animal = (int)dr["ID_Animal"],
                        Fecha = (System.DateTime)dr["Fecha"],
                        Tipo = dr["Tipo"].ToString(),
                        Detalle = dr["Detalle"].ToString()
                    });
                }
            }
            ViewBag.ID_Animal = idAnimal;
            return View(lista);
        }

        // Crear historial
        public ActionResult Crear(int idAnimal)
        {
            return View(new HistorialMedicoModel { ID_Animal = idAnimal });
        }

        [HttpPost]
        public ActionResult Crear(HistorialMedicoModel model)
        {
            using (SqlConnection cn = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Historial_Medico (ID_Animal, Tipo, Detalle) VALUES (@id, @tipo, @detalle)", cn);
                cmd.Parameters.AddWithValue("@id", model.ID_Animal);
                cmd.Parameters.AddWithValue("@tipo", model.Tipo);
                cmd.Parameters.AddWithValue("@detalle", model.Detalle);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index", new { idAnimal = model.ID_Animal });
        }
    }
}
