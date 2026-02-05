using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using AdoptameLiberia.Models.TiposAnimales;

namespace AdoptameLiberia.Controllers.TiposAnimales
{
    public class TipoAnimalController : Controller
    {
        private readonly string connectionString;

        public TipoAnimalController()
        {
            connectionString = ConfigurationManager
                .ConnectionStrings["DefaultConnection"]
                .ConnectionString;
        }

        public ActionResult Index()
        {
            List<TipoAnimal> lista = new List<TipoAnimal>();

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                string sql = "SELECT ID_TipoAnimal, Nombre_Tipo_Animal, Descripcion, Estado FROM Tipo_Animal";

                SqlCommand comando = new SqlCommand(sql, conexion);
                conexion.Open();

                SqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    TipoAnimal tipo = new TipoAnimal();
                    tipo.ID_TipoAnimal = reader.GetInt32(0);
                    tipo.Nombre_Tipo_Animal = reader.GetString(1);
                    tipo.Descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    tipo.Estado = reader.GetBoolean(3);

                    lista.Add(tipo);
                }
            }

            return View(lista);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(TipoAnimal modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                string sql = @"INSERT INTO Tipo_Animal 
                               (Nombre_Tipo_Animal, Descripcion, Estado)
                               VALUES (@Nombre, @Descripcion, 1)";

                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@Nombre", modelo.Nombre_Tipo_Animal);
                comando.Parameters.AddWithValue("@Descripcion", modelo.Descripcion);

                conexion.Open();
                comando.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            TipoAnimal tipo = new TipoAnimal();

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                string sql = @"SELECT ID_TipoAnimal, Nombre_Tipo_Animal, Descripcion, Estado 
                               FROM Tipo_Animal WHERE ID_TipoAnimal = @Id";

                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@Id", id);

                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();

                if (reader.Read())
                {
                    tipo.ID_TipoAnimal = reader.GetInt32(0);
                    tipo.Nombre_Tipo_Animal = reader.GetString(1);
                    tipo.Descripcion = reader.IsDBNull(2) ? "" : reader.GetString(2);
                    tipo.Estado = reader.GetBoolean(3);
                }
            }

            return View(tipo);
        }

        [HttpPost]
        public ActionResult Edit(TipoAnimal modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                string sql = @"UPDATE Tipo_Animal 
                               SET Nombre_Tipo_Animal = @Nombre,
                                   Descripcion = @Descripcion,
                                   Estado = @Estado
                               WHERE ID_TipoAnimal = @Id";

                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@Nombre", modelo.Nombre_Tipo_Animal);
                comando.Parameters.AddWithValue("@Descripcion", modelo.Descripcion);
                comando.Parameters.AddWithValue("@Estado", modelo.Estado);
                comando.Parameters.AddWithValue("@Id", modelo.ID_TipoAnimal);

                conexion.Open();
                comando.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            TipoAnimal tipo = new TipoAnimal();
            tipo.ID_TipoAnimal = id;
            return View(tipo);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                string sql = @"UPDATE Tipo_Animal 
                               SET Estado = 0 
                               WHERE ID_TipoAnimal = @Id";

                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@Id", id);

                conexion.Open();
                comando.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}
