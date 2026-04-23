using AdoptameLiberia.Models.Mascotas;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.IO;
using System.Linq;
using AdoptameLiberia.Models.ViewModel;

namespace AdoptameLiberia.Controllers.Mascotas
{
    public class AnimalController : Controller
    {
        private readonly string conexion = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Catalogo()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { ReturnUrl = Url.Action("Catalogo", "Animal") });
            }

            List<AnimalModel> lista = new List<AnimalModel>();
            HashSet<int> favoritos = new HashSet<int>();
            string currentUserId = User.Identity.GetUserId();

            string filtroEstado = User.IsInRole("Administrador")
                ? "WHERE a.Estado IS NULL OR a.Estado <> 'Inactivo'"
                : "WHERE a.Estado = 'Disponible'";

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sql = $@"
        SELECT 
            a.ID_Animal,
            a.Nombre_Animal,
            a.ID_Raza,
            a.ID_TipoAnimal,
            a.Edad,
            a.Sexo,
            a.Tamano,
            a.Peso,
            a.Descripcion,
            a.Estado,
            a.UsuarioRegistroId,
            a.ImagenUrl,
            ISNULL(r.Nombre, a.NombreRaza) AS NombreRaza,
            ISNULL(t.Nombre_Tipo_Animal, a.NombreTipo) AS NombreTipo
        FROM Animal a
        LEFT JOIN Raza r ON a.ID_Raza = r.ID_Raza
        LEFT JOIN Tipo_Animal t ON a.ID_TipoAnimal = t.ID_TipoAnimal
        {filtroEstado}
        ORDER BY a.ID_Animal DESC";

                SqlCommand cmd = new SqlCommand(sql, cn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new AnimalModel
                    {
                        ID_Animal = Convert.ToInt32(dr["ID_Animal"]),
                        Nombre_Animal = dr["Nombre_Animal"]?.ToString(),
                        ID_Raza = dr["ID_Raza"] != DBNull.Value ? Convert.ToInt32(dr["ID_Raza"]) : 0,
                        ID_TipoAnimal = dr["ID_TipoAnimal"] != DBNull.Value ? Convert.ToInt32(dr["ID_TipoAnimal"]) : 0,
                        Edad = dr["Edad"] != DBNull.Value ? Convert.ToInt32(dr["Edad"]) : (int?)null,
                        Sexo = dr["Sexo"]?.ToString(),
                        Tamano = dr["Tamano"]?.ToString(),
                        Peso = dr["Peso"] != DBNull.Value ? Convert.ToDecimal(dr["Peso"]) : (decimal?)null,
                        Descripcion = dr["Descripcion"]?.ToString(),
                        Estado = dr["Estado"]?.ToString(),
                        NombreRaza = dr["NombreRaza"]?.ToString(),
                        NombreTipo = dr["NombreTipo"]?.ToString(),
                        UsuarioRegistroId = dr["UsuarioRegistroId"]?.ToString(),
                        ImagenUrl = dr["ImagenUrl"]?.ToString()
                    });
                }
            }

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sqlFavoritos = @"
        SELECT ID_Animal
        FROM Favorito
        WHERE UserId = @UserId";

                SqlCommand cmdFavoritos = new SqlCommand(sqlFavoritos, cn);
                cmdFavoritos.Parameters.AddWithValue("@UserId", (object)currentUserId ?? DBNull.Value);

                SqlDataReader drFavoritos = cmdFavoritos.ExecuteReader();
                while (drFavoritos.Read())
                {
                    favoritos.Add(Convert.ToInt32(drFavoritos["ID_Animal"]));
                }
            }

            ViewBag.Favoritos = favoritos;
            ViewBag.EsAdmin = User.IsInRole("Administrador");

            return View(lista);
        }

        public ActionResult MascotaIdeal(string tamano, int? idTipoAnimal, int? idRaza, int? edad, string personalidad)
        {
            List<AnimalModel> lista = new List<AnimalModel>();

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sql = @"
        SELECT 
            a.ID_Animal,
            a.Nombre_Animal,
            a.ID_Raza,
            a.ID_TipoAnimal,
            a.Edad,
            a.Sexo,
            a.Tamano,
            a.Peso,
            a.Descripcion,
            a.Estado,
            a.UsuarioRegistroId,
            a.ImagenUrl,
            ISNULL(r.Nombre, a.NombreRaza) AS NombreRaza,
            ISNULL(t.Nombre_Tipo_Animal, a.NombreTipo) AS NombreTipo
        FROM Animal a
        LEFT JOIN Raza r ON a.ID_Raza = r.ID_Raza
        LEFT JOIN Tipo_Animal t ON a.ID_TipoAnimal = t.ID_TipoAnimal
        WHERE a.Estado = 'Disponible'
        ORDER BY a.ID_Animal DESC";

                SqlCommand cmd = new SqlCommand(sql, cn);
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    lista.Add(new AnimalModel
                    {
                        ID_Animal = Convert.ToInt32(dr["ID_Animal"]),
                        Nombre_Animal = dr["Nombre_Animal"]?.ToString(),
                        ID_Raza = dr["ID_Raza"] != DBNull.Value ? Convert.ToInt32(dr["ID_Raza"]) : 0,
                        ID_TipoAnimal = dr["ID_TipoAnimal"] != DBNull.Value ? Convert.ToInt32(dr["ID_TipoAnimal"]) : 0,
                        Edad = dr["Edad"] != DBNull.Value ? Convert.ToInt32(dr["Edad"]) : (int?)null,
                        Sexo = dr["Sexo"]?.ToString(),
                        Tamano = dr["Tamano"]?.ToString(),
                        Peso = dr["Peso"] != DBNull.Value ? Convert.ToDecimal(dr["Peso"]) : (decimal?)null,
                        Descripcion = dr["Descripcion"]?.ToString(),
                        Estado = dr["Estado"]?.ToString(),
                        NombreRaza = dr["NombreRaza"]?.ToString(),
                        NombreTipo = dr["NombreTipo"]?.ToString(),
                        UsuarioRegistroId = dr["UsuarioRegistroId"]?.ToString(),
                        ImagenUrl = dr["ImagenUrl"]?.ToString()
                    });
                }
            }

            if (idTipoAnimal.HasValue && idTipoAnimal.Value > 0)
            {
                lista = lista.Where(x => x.ID_TipoAnimal == idTipoAnimal.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(tamano))
            {
                lista = lista
                    .Where(x => !string.IsNullOrWhiteSpace(x.Tamano) &&
                                x.Tamano.Trim().ToLower() == tamano.Trim().ToLower())
                    .ToList();
            }

            if (idRaza.HasValue && idRaza.Value > 0)
            {
                lista = lista.Where(x => x.ID_Raza == idRaza.Value).ToList();
            }

            if (edad.HasValue)
            {
                lista = lista
                    .Where(x => x.Edad.HasValue && x.Edad.Value <= edad.Value)
                    .ToList();
            }

            if (!string.IsNullOrWhiteSpace(personalidad))
            {
                string personalidadFiltro = personalidad.Trim().ToLower();

                lista = lista
                    .Where(x =>
                        (!string.IsNullOrWhiteSpace(x.Descripcion) && x.Descripcion.ToLower().Contains(personalidadFiltro)) ||
                        (!string.IsNullOrWhiteSpace(x.Nombre_Animal) && x.Nombre_Animal.ToLower().Contains(personalidadFiltro)) ||
                        (!string.IsNullOrWhiteSpace(x.NombreRaza) && x.NombreRaza.ToLower().Contains(personalidadFiltro))
                    )
                    .ToList();
            }

            ViewBag.ListaTiposFiltro = ObtenerTiposParaFiltro(idTipoAnimal);
            ViewBag.ListaRazasFiltro = ObtenerRazasParaFiltro(idRaza);
            ViewBag.TamanoFiltro = tamano;
            ViewBag.EdadFiltro = edad;
            ViewBag.PersonalidadFiltro = personalidad;

            return View(lista);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Crear()
        {
            CargarCatalogos();
            return View(new RegistrarAnimalVM());
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Crear(RegistrarAnimalVM model)
        {
            var animal = new AnimalModel
            {
                Nombre_Animal = model.Nombre_Animal,
                ID_Raza = model.ID_Raza,
                ID_TipoAnimal = model.ID_TipoAnimal,
                Edad = model.Edad,
                Sexo = model.Sexo,
                Tamano = model.Tamano,
                Peso = model.Peso,
                Descripcion = model.Descripcion
            };

            ValidarCatalogos(animal);

            if (!ModelState.IsValid)
            {
                CargarCatalogos(model.ID_Raza, model.ID_TipoAnimal);
                return View(model);
            }

            string rutaImagen = null;

            if (model.ImagenArchivo != null && model.ImagenArchivo.ContentLength > 0)
            {
                string extension = Path.GetExtension(model.ImagenArchivo.FileName).ToLower();
                string[] permitidas = { ".jpg", ".jpeg", ".png", ".webp" };

                if (!permitidas.Contains(extension))
                {
                    ModelState.AddModelError("", "Formato de imagen no válido.");
                    CargarCatalogos(model.ID_Raza, model.ID_TipoAnimal);
                    return View(model);
                }

                if (model.ImagenArchivo.ContentLength > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("", "La imagen no puede superar los 5MB.");
                    CargarCatalogos(model.ID_Raza, model.ID_TipoAnimal);
                    return View(model);
                }

                string nombreArchivo = Guid.NewGuid().ToString() + extension;
                string carpeta = Server.MapPath("~/Content/img/mascotas/");
                string rutaCompleta = Path.Combine(carpeta, nombreArchivo);

                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                model.ImagenArchivo.SaveAs(rutaCompleta);
                rutaImagen = "/Content/img/mascotas/" + nombreArchivo;
            }

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                string sql = @"
        INSERT INTO Animal
        (
            Nombre_Animal,
            ID_Raza,
            ID_TipoAnimal,
            Edad,
            Sexo,
            Tamano,
            Peso,
            Descripcion,
            Estado,
            UsuarioRegistroId,
            ImagenUrl
        )
        VALUES
        (
            @Nombre_Animal,
            @ID_Raza,
            @ID_TipoAnimal,
            @Edad,
            @Sexo,
            @Tamano,
            @Peso,
            @Descripcion,
            'Disponible',
            @UsuarioRegistroId,
            @ImagenUrl
        )";

                SqlCommand cmd = new SqlCommand(sql, cn);

                cmd.Parameters.AddWithValue("@Nombre_Animal", (object)model.Nombre_Animal ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_Raza", model.ID_Raza);
                cmd.Parameters.AddWithValue("@ID_TipoAnimal", model.ID_TipoAnimal);
                cmd.Parameters.AddWithValue("@Edad", (object)model.Edad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Sexo", (object)model.Sexo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Tamano", (object)model.Tamano ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Peso", (object)model.Peso ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", (object)model.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@UsuarioRegistroId", User.Identity.GetUserId());
                cmd.Parameters.AddWithValue("@ImagenUrl", (object)rutaImagen ?? DBNull.Value);

                cn.Open();
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Catalogo");
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Editar(int id)
        {
            AnimalModel model = null;

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                string sql = @"
                SELECT 
                    ID_Animal,
                    Nombre_Animal,
                    ID_Raza,
                    ID_TipoAnimal,
                    Edad,
                    Sexo,
                    Tamano,
                    Peso,
                    Descripcion,
                    Estado,
                    UsuarioRegistroId
                FROM Animal
                WHERE ID_Animal = @id";

                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@id", id);

                cn.Open();
                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    model = new AnimalModel
                    {
                        ID_Animal = Convert.ToInt32(dr["ID_Animal"]),
                        Nombre_Animal = dr["Nombre_Animal"]?.ToString(),
                        ID_Raza = dr["ID_Raza"] != DBNull.Value ? Convert.ToInt32(dr["ID_Raza"]) : 0,
                        ID_TipoAnimal = dr["ID_TipoAnimal"] != DBNull.Value ? Convert.ToInt32(dr["ID_TipoAnimal"]) : 0,
                        Edad = dr["Edad"] != DBNull.Value ? Convert.ToInt32(dr["Edad"]) : (int?)null,
                        Sexo = dr["Sexo"]?.ToString(),
                        Tamano = dr["Tamano"]?.ToString(),
                        Peso = dr["Peso"] != DBNull.Value ? Convert.ToDecimal(dr["Peso"]) : (decimal?)null,
                        Descripcion = dr["Descripcion"]?.ToString(),
                        Estado = dr["Estado"]?.ToString(),
                        UsuarioRegistroId = dr["UsuarioRegistroId"]?.ToString()
                    };
                }
            }

            if (model == null)
            {
                return HttpNotFound();
            }

            CargarCatalogos(model.ID_Raza, model.ID_TipoAnimal);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(AnimalModel model)
        {
            ValidarCatalogos(model);

            if (!ModelState.IsValid)
            {
                CargarCatalogos(model.ID_Raza, model.ID_TipoAnimal);
                return View(model);
            }

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sql = @"
                UPDATE Animal
                SET
                    Nombre_Animal = @Nombre_Animal,
                    ID_Raza = @ID_Raza,
                    ID_TipoAnimal = @ID_TipoAnimal,
                    Edad = @Edad,
                    Sexo = @Sexo,
                    Tamano = @Tamano,
                    Peso = @Peso,
                    Descripcion = @Descripcion
                WHERE ID_Animal = @ID_Animal";

                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@Nombre_Animal", (object)model.Nombre_Animal ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_Raza", model.ID_Raza);
                cmd.Parameters.AddWithValue("@ID_TipoAnimal", model.ID_TipoAnimal);
                cmd.Parameters.AddWithValue("@Edad", (object)model.Edad ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Sexo", (object)model.Sexo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Tamano", (object)model.Tamano ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Peso", (object)model.Peso ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Descripcion", (object)model.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@ID_Animal", model.ID_Animal);

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Catalogo");
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Desactivar(int id)
        {
            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sql = "UPDATE Animal SET Estado = 'Inactivo' WHERE ID_Animal = @ID_Animal";
                SqlCommand cmd = new SqlCommand(sql, cn);
                cmd.Parameters.AddWithValue("@ID_Animal", id);
                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Catalogo");
        }

        private void ValidarCatalogos(AnimalModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Nombre_Animal))
            {
                ModelState.AddModelError("Nombre_Animal", "Debes ingresar el nombre de la mascota.");
            }

            if (model.ID_Raza <= 0)
            {
                ModelState.AddModelError("ID_Raza", "Debes seleccionar una raza.");
            }

            if (model.ID_TipoAnimal <= 0)
            {
                ModelState.AddModelError("ID_TipoAnimal", "Debes seleccionar un tipo de animal.");
            }
        }

        private void CargarCatalogos(int? razaSeleccionada = null, int? tipoSeleccionado = null)
        {
            var razas = new List<SelectListItem>();
            var tipos = new List<SelectListItem>();

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sqlRazas = "SELECT ID_Raza, Nombre FROM Raza ORDER BY Nombre";
                using (SqlCommand cmd = new SqlCommand(sqlRazas, cn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int id = Convert.ToInt32(dr["ID_Raza"]);
                        string nombre = dr["Nombre"].ToString();

                        razas.Add(new SelectListItem
                        {
                            Value = id.ToString(),
                            Text = nombre,
                            Selected = razaSeleccionada.HasValue && razaSeleccionada.Value == id
                        });
                    }
                }

                string sqlTipos = "SELECT ID_TipoAnimal, Nombre_Tipo_Animal FROM Tipo_Animal ORDER BY ID_TipoAnimal";
                using (SqlCommand cmd = new SqlCommand(sqlTipos, cn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int id = Convert.ToInt32(dr["ID_TipoAnimal"]);
                        string nombre = dr["Nombre_Tipo_Animal"].ToString();

                        tipos.Add(new SelectListItem
                        {
                            Value = id.ToString(),
                            Text = nombre,
                            Selected = tipoSeleccionado.HasValue && tipoSeleccionado.Value == id
                        });
                    }
                }
            }

            ViewBag.ListaRazas = razas;
            ViewBag.ListaTipos = tipos;
        }

        private List<SelectListItem> ObtenerRazasParaFiltro(int? razaSeleccionada = null)
        {
            var razas = new List<SelectListItem>();

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sqlRazas = "SELECT ID_Raza, Nombre FROM Raza ORDER BY Nombre";
                using (SqlCommand cmd = new SqlCommand(sqlRazas, cn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int id = Convert.ToInt32(dr["ID_Raza"]);
                        string nombre = dr["Nombre"].ToString();

                        razas.Add(new SelectListItem
                        {
                            Value = id.ToString(),
                            Text = nombre,
                            Selected = razaSeleccionada.HasValue && razaSeleccionada.Value == id
                        });
                    }
                }
            }

            return razas;
        }

        private List<SelectListItem> ObtenerTiposParaFiltro(int? tipoSeleccionado = null)
        {
            var tipos = new List<SelectListItem>();

            using (SqlConnection cn = new SqlConnection(conexion))
            {
                cn.Open();

                string sqlTipos = "SELECT ID_TipoAnimal, Nombre_Tipo_Animal FROM Tipo_Animal ORDER BY Nombre_Tipo_Animal";
                using (SqlCommand cmd = new SqlCommand(sqlTipos, cn))
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        int id = Convert.ToInt32(dr["ID_TipoAnimal"]);
                        string nombre = dr["Nombre_Tipo_Animal"].ToString();

                        tipos.Add(new SelectListItem
                        {
                            Value = id.ToString(),
                            Text = nombre,
                            Selected = tipoSeleccionado.HasValue && tipoSeleccionado.Value == id
                        });
                    }
                }
            }

            return tipos;
        }
    }
}