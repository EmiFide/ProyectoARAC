using AdoptameLiberia.Models.Mascotas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace AdoptameLiberia.Controllers.Mascotas
{
    [Authorize]
    public class AnimalController : Controller
    {
        string conexion = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        // H15 - Catálogo
        public ActionResult Catalogo()
        {
            List<AnimalModel> lista = new List<AnimalModel>();

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                string sql = @"
                SELECT a.ID_Animal, a.Nombre_Animal, a.Edad, a.Sexo, a.Tamano, a.Peso,
                       a.Descripcion, a.Estado,
                       r.NombreRaza AS NombreRaza,
                       t.Nombre_Tipo_Animal AS NombreTipo
                FROM Animal a
                LEFT JOIN Razas r ON a.ID_Raza = r.ID_Raza
                LEFT JOIN Tipo_Animal t ON a.ID_TipoAnimal = t.ID_TipoAnimal
                WHERE a.Estado = 'Disponible'";

                SqlCommand cmd = new SqlCommand(sql, cn);
                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new AnimalModel
                    {
                        ID_Animal = (int)dr["ID_Animal"],
                        Nombre_Animal = dr["Nombre_Animal"].ToString(),
                        Edad = dr["Edad"] as int?,
                        Sexo = dr["Sexo"].ToString(),
                        Tamano = dr["Tamano"].ToString(),
                        Peso = dr["Peso"] as decimal?,
                        Descripcion = dr["Descripcion"].ToString(),
                        Estado = dr["Estado"].ToString(),
                        NombreRaza = dr["NombreRaza"].ToString(),
                        NombreTipo = dr["NombreTipo"].ToString()
                    });
                }
            }
            return View(lista);
        }

        // H13 - Crear
        public ActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Crear(AnimalModel model)
        {
            using (SqlConnection cn = new SqlConnection(conexion))
            {
                string sql = @"INSERT INTO Animal
                (Nombre_Animal, ID_Raza, ID_TipoAnimal, Edad, Sexo, Tamano, Peso, Descripcion, Estado)
                VALUES
                (@Nombre, @ID_Raza, @ID_Tipo, @Edad, @Sexo, @Tamano, @Peso, @Descripcion, 'Disponible')";

                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@Nombre", model.Nombre_Animal);
                cmd.Parameters.AddWithValue("@ID_Raza", model.ID_Raza);
                cmd.Parameters.AddWithValue("@ID_Tipo", model.ID_TipoAnimal);
                cmd.Parameters.AddWithValue("@Edad", (object)model.Edad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Sexo", model.Sexo);
                cmd.Parameters.AddWithValue("@Tamano", model.Tamano);
                cmd.Parameters.AddWithValue("@Peso", (object)model.Peso ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", model.Descripcion);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Catalogo");
        }

        // H14 - Editar
        public ActionResult Editar(int id)
        {
            AnimalModel model = new AnimalModel();

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                string sql = "SELECT * FROM Animal WHERE ID_Animal=@id";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@id", id);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    model.ID_Animal = (int)dr["ID_Animal"];
                    model.Nombre_Animal = dr["Nombre_Animal"].ToString();
                    model.ID_Raza = (int)dr["ID_Raza"];
                    model.ID_TipoAnimal = (int)dr["ID_TipoAnimal"];
                    model.Edad = dr["Edad"] as int?;
                    model.Sexo = dr["Sexo"].ToString();
                    model.Tamano = dr["Tamano"].ToString();
                    model.Peso = dr["Peso"] as decimal?;
                    model.Descripcion = dr["Descripcion"].ToString();
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Editar(AnimalModel model)
        {
            using (SqlConnection cn = new SqlConnection(conexion))
            {
                string sql = @"UPDATE Animal SET
                Nombre_Animal=@Nombre,
                ID_Raza=@ID_Raza,
                ID_TipoAnimal=@ID_Tipo,
                Edad=@Edad,
                Sexo=@Sexo,
                Tamano=@Tamano,
                Peso=@Peso,
                Descripcion=@Descripcion
                WHERE ID_Animal=@ID";

                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@Nombre", model.Nombre_Animal);
                cmd.Parameters.AddWithValue("@ID_Raza", model.ID_Raza);
                cmd.Parameters.AddWithValue("@ID_Tipo", model.ID_TipoAnimal);
                cmd.Parameters.AddWithValue("@Edad", (object)model.Edad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Sexo", model.Sexo);
                cmd.Parameters.AddWithValue("@Tamano", model.Tamano);
                cmd.Parameters.AddWithValue("@Peso", (object)model.Peso ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", model.Descripcion);
                cmd.Parameters.AddWithValue("@ID", model.ID_Animal);

                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Catalogo");
        }

        // H17 - Eliminación lógica
        public ActionResult Desactivar(int id)
        {
            using (SqlConnection cn = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand(
                    "UPDATE Animal SET Estado='Inactivo' WHERE ID_Animal=@id", cn);
                cmd.Parameters.AddWithValue("@id", id);
                cn.Open();
                cmd.ExecuteNonQuery();
            }
            return RedirectToAction("Catalogo");
        }
    }
}
