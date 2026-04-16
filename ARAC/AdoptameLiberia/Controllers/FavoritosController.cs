using AdoptameLiberia.Models.Mascotas;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace AdoptameLiberia.Controllers
{
    [Authorize]
    public class FavoritosController : Controller
    {
        private readonly string conexion = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            var lista = new List<AnimalModel>();
            string userId = User.Identity.GetUserId();

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                string sql = @"
                SELECT 
                    a.ID_Animal,
                    a.Nombre_Animal,
                    a.Edad,
                    a.Sexo,
                    a.Tamano,
                    a.Peso,
                    a.Descripcion,
                    a.Estado,
                    a.ImagenUrl,
                    ISNULL(r.Nombre, a.NombreRaza) AS NombreRaza,
                    ISNULL(t.Nombre_Tipo_Animal, a.NombreTipo) AS NombreTipo
                FROM Favorito f
                INNER JOIN Animal a ON a.ID_Animal = f.ID_Animal
                LEFT JOIN Raza r ON a.ID_Raza = r.ID_Raza
                LEFT JOIN Tipo_Animal t ON a.ID_TipoAnimal = t.ID_TipoAnimal
                WHERE 
                    f.UserId = @UserId
                    AND (a.Estado IS NULL OR a.Estado <> 'Inactivo')
                ORDER BY f.Fecha_Registro DESC, a.Nombre_Animal ASC";

                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@UserId", userId);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new AnimalModel
                    {
                        ID_Animal = Convert.ToInt32(dr["ID_Animal"]),
                        Nombre_Animal = dr["Nombre_Animal"]?.ToString(),
                        Edad = dr["Edad"] != DBNull.Value ? Convert.ToInt32(dr["Edad"]) : (int?)null,
                        Sexo = dr["Sexo"]?.ToString(),
                        Tamano = dr["Tamano"]?.ToString(),
                        Peso = dr["Peso"] != DBNull.Value ? Convert.ToDecimal(dr["Peso"]) : (decimal?)null,
                        Descripcion = dr["Descripcion"]?.ToString(),
                        Estado = dr["Estado"]?.ToString(),
                        NombreRaza = dr["NombreRaza"]?.ToString(),
                        ImagenUrl = dr["ImagenUrl"]?.ToString(),
                        NombreTipo = dr["NombreTipo"]?.ToString()
                    });
                }
            }

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Toggle(int idAnimal, string returnUrl)
        {
            string userId = User.Identity.GetUserId();

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sqlValidacion = @"
                SELECT COUNT(1)
                FROM Animal
                WHERE ID_Animal = @ID_Animal
                AND (Estado IS NULL OR Estado <> 'Inactivo')";

                SqlCommand cmdValidacion = new SqlCommand(sqlValidacion, cn);
                cmdValidacion.Parameters.AddWithValue("@ID_Animal", idAnimal);

                bool permitido = Convert.ToInt32(cmdValidacion.ExecuteScalar()) > 0;

                if (!permitido)
                {
                    return new HttpUnauthorizedResult();
                }

                string existeSql = "SELECT COUNT(1) FROM Favorito WHERE UserId = @UserId AND ID_Animal = @ID_Animal";
                SqlCommand existeCmd = new SqlCommand(existeSql, cn);
                existeCmd.Parameters.AddWithValue("@UserId", userId);
                existeCmd.Parameters.AddWithValue("@ID_Animal", idAnimal);

                bool existe = Convert.ToInt32(existeCmd.ExecuteScalar()) > 0;

                if (existe)
                {
                    string deleteSql = "DELETE FROM Favorito WHERE UserId = @UserId AND ID_Animal = @ID_Animal";
                    SqlCommand deleteCmd = new SqlCommand(deleteSql, cn);
                    deleteCmd.Parameters.AddWithValue("@UserId", userId);
                    deleteCmd.Parameters.AddWithValue("@ID_Animal", idAnimal);
                    deleteCmd.ExecuteNonQuery();
                }
                else
                {
                    string insertSql = @"
                    INSERT INTO Favorito (UserId, ID_Animal, Fecha_Registro)
                    VALUES (@UserId, @ID_Animal, GETDATE())";

                    SqlCommand insertCmd = new SqlCommand(insertSql, cn);
                    insertCmd.Parameters.AddWithValue("@UserId", userId);
                    insertCmd.Parameters.AddWithValue("@ID_Animal", idAnimal);
                    insertCmd.ExecuteNonQuery();
                }
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Catalogo", "Animal");
        }
    }
}