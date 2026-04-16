using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Web.Mvc;
using AdoptameLiberia.Models;

namespace AdoptameLiberia.Controllers
{
    public class RazasController : Controller
    {
        private readonly string connectionString;

        public RazasController()
        {
            connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        // GET: Razas
        public ActionResult Index(string orden)
        {
            List<Raza> lista = new List<Raza>();

            ViewBag.OrdenId = orden == "id_asc" ? "id_desc" : "id_asc";
            ViewBag.OrdenNombre = orden == "nombre_asc" ? "nombre_desc" : "nombre_asc";

            string orderBy = " ORDER BY ID_Raza ASC ";

            if (orden == "id_desc")
            {
                orderBy = " ORDER BY ID_Raza DESC ";
            }
            else if (orden == "nombre_asc")
            {
                orderBy = " ORDER BY Nombre ASC ";
            }
            else if (orden == "nombre_desc")
            {
                orderBy = " ORDER BY Nombre DESC ";
            }

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                string sql = "SELECT ID_Raza, Nombre, Descripcion FROM Raza" + orderBy;

                SqlCommand comando = new SqlCommand(sql, conexion);
                conexion.Open();

                SqlDataReader reader = comando.ExecuteReader();

                while (reader.Read())
                {
                    Raza raza = new Raza();
                    raza.ID_Raza = Convert.ToInt32(reader["ID_Raza"]);
                    raza.Nombre = reader["Nombre"] == DBNull.Value ? "" : reader["Nombre"].ToString();
                    raza.Descripcion = reader["Descripcion"] == DBNull.Value ? "" : reader["Descripcion"].ToString();

                    lista.Add(raza);
                }
            }

            return View(lista);
        }

        // GET: Razas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Razas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Raza raza)
        {
            if (string.IsNullOrWhiteSpace(raza.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre de la raza es obligatorio.");
            }

            if (ExisteRaza(raza.Nombre))
            {
                ModelState.AddModelError("Nombre", "Ya existe una raza con ese nombre.");
            }

            if (!ModelState.IsValid)
            {
                return View(raza);
            }

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                string sql = "INSERT INTO Raza (Nombre, Descripcion) VALUES (@Nombre, @Descripcion)";

                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@Nombre", raza.Nombre.Trim());

                if (string.IsNullOrWhiteSpace(raza.Descripcion))
                {
                    comando.Parameters.AddWithValue("@Descripcion", DBNull.Value);
                }
                else
                {
                    comando.Parameters.AddWithValue("@Descripcion", raza.Descripcion.Trim());
                }

                conexion.Open();
                comando.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // GET: Razas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Raza raza = ObtenerRazaPorId(id.Value);

            if (raza == null)
            {
                return HttpNotFound();
            }

            return View(raza);
        }

        // POST: Razas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Raza raza)
        {
            if (string.IsNullOrWhiteSpace(raza.Nombre))
            {
                ModelState.AddModelError("Nombre", "El nombre de la raza es obligatorio.");
            }

            if (ExisteRaza(raza.Nombre, raza.ID_Raza))
            {
                ModelState.AddModelError("Nombre", "Ya existe otra raza con ese nombre.");
            }

            if (!ModelState.IsValid)
            {
                return View(raza);
            }

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                string sql = "UPDATE Raza SET Nombre = @Nombre, Descripcion = @Descripcion WHERE ID_Raza = @ID_Raza";

                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@ID_Raza", raza.ID_Raza);
                comando.Parameters.AddWithValue("@Nombre", raza.Nombre.Trim());

                if (string.IsNullOrWhiteSpace(raza.Descripcion))
                {
                    comando.Parameters.AddWithValue("@Descripcion", DBNull.Value);
                }
                else
                {
                    comando.Parameters.AddWithValue("@Descripcion", raza.Descripcion.Trim());
                }

                conexion.Open();
                comando.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        // GET: Razas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Raza raza = ObtenerRazaPorId(id.Value);

            if (raza == null)
            {
                return HttpNotFound();
            }

            return View(raza);
        }

        // POST: Razas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                string sql = "DELETE FROM Raza WHERE ID_Raza = @ID_Raza";

                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@ID_Raza", id);

                conexion.Open();
                comando.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }

        private Raza ObtenerRazaPorId(int id)
        {
            Raza raza = null;

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                string sql = "SELECT ID_Raza, Nombre, Descripcion FROM Raza WHERE ID_Raza = @ID_Raza";

                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@ID_Raza", id);

                conexion.Open();
                SqlDataReader reader = comando.ExecuteReader();

                if (reader.Read())
                {
                    raza = new Raza();
                    raza.ID_Raza = Convert.ToInt32(reader["ID_Raza"]);
                    raza.Nombre = reader["Nombre"] == DBNull.Value ? "" : reader["Nombre"].ToString();
                    raza.Descripcion = reader["Descripcion"] == DBNull.Value ? "" : reader["Descripcion"].ToString();
                }
            }

            return raza;
        }

        private bool ExisteRaza(string nombre, int? idExcluir = null)
        {
            bool existe = false;

            using (SqlConnection conexion = new SqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Raza WHERE UPPER(LTRIM(RTRIM(Nombre))) = UPPER(LTRIM(RTRIM(@Nombre)))";

                if (idExcluir.HasValue)
                {
                    sql += " AND ID_Raza <> @ID_Raza";
                }

                SqlCommand comando = new SqlCommand(sql, conexion);
                comando.Parameters.AddWithValue("@Nombre", nombre ?? "");

                if (idExcluir.HasValue)
                {
                    comando.Parameters.AddWithValue("@ID_Raza", idExcluir.Value);
                }

                conexion.Open();
                int cantidad = (int)comando.ExecuteScalar();
                existe = cantidad > 0;
            }

            return existe;
        }
    }
}