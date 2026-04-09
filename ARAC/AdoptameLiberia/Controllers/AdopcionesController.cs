using AdoptameLiberia.Models;
using AdoptameLiberia.Models.Mascotas;
using AdoptameLiberia.Models.ViewModel;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace AdoptameLiberia.Controllers
{
    [Authorize]
    public class AdopcionesController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        private bool EsAdministrador()
        {
            return User.IsInRole("Administrador");
        }

        private int? ObtenerIdUsuarioActual()
        {
            string aspNetUserId = User.Identity.GetUserId();

            return db.Usuarios
                     .Where(u => u.IdAspNetUser == aspNetUserId)
                     .Select(u => (int?)u.ID_Usuario)
                     .FirstOrDefault();
        }

        private string NombreCompletoUsuario(Usuario u)
        {
            if (u == null) return "";
            return string.Format("{0} {1} {2}", u.Nombre, u.Apellido1, u.Apellido2).Trim();
        }

        private void CargarUsuarios(int? seleccionado = null)
        {
            var usuarios = db.Usuarios
                .ToList()
                .Select(u => new
                {
                    u.ID_Usuario,
                    NombreCompleto = (u.Nombre + " " + u.Apellido1 + " " + u.Apellido2).Trim()
                })
                .OrderBy(x => x.NombreCompleto)
                .ToList();

            ViewBag.ID_Usuario = new SelectList(usuarios, "ID_Usuario", "NombreCompleto", seleccionado);
        }

        private void CargarAnimalesDisponibles(int? seleccionado = null)
        {
            var animales = db.Animals
                .Where(a => a.Estado == "Disponible")
                .OrderBy(a => a.Nombre_Animal)
                .ToList();

            ViewBag.ID_Animal = new SelectList(animales, "ID_Animal", "Nombre_Animal", seleccionado);
        }

        public ActionResult Index()
        {
            var adopciones = (from a in db.Adopcion
                              join s in db.SolicitudAdopcion on a.ID_Solicitud equals s.ID_Solicitud
                              join an in db.Animals on a.ID_Animal equals an.ID_Animal
                              join u in db.Usuarios on s.ID_Usuario equals u.ID_Usuario into uj
                              from u in uj.DefaultIfEmpty()
                              select new AdopcionResumenVM
                              {
                                  ID_Adopcion = a.ID_Adopcion,
                                  ID_Solicitud = a.ID_Solicitud,
                                  AnimalNombre = an.Nombre_Animal,
                                  UsuarioNombre = u == null ? "" : (u.Nombre + " " + u.Apellido1 + " " + u.Apellido2).Trim(),
                                  Fecha_Adopcion = a.Fecha_Adopcion,
                                  Estado_Adopcion = a.Estado_Adopcion,
                                  Seguimiento_Inicial = a.Seguimiento_Inicial
                              }).ToList();

            return View(adopciones);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Solicitudes()
        {
            var lista = (from s in db.SolicitudAdopcion
                         join u in db.Usuarios on s.ID_Usuario equals u.ID_Usuario into uj
                         from u in uj.DefaultIfEmpty()
                         join a in db.Animals on s.ID_Animal equals a.ID_Animal into aj
                         from a in aj.DefaultIfEmpty()
                         orderby s.Fecha_Solicitud descending
                         select new SolicitudResumenVM
                         {
                             ID_Solicitud = s.ID_Solicitud,
                             ID_Usuario = s.ID_Usuario,
                             UsuarioNombre = u == null ? "" : (u.Nombre + " " + u.Apellido1 + " " + u.Apellido2).Trim(),
                             ID_Animal = s.ID_Animal,
                             AnimalNombre = a == null ? "" : a.Nombre_Animal,
                             Fecha_Solicitud = s.Fecha_Solicitud,
                             Estado = s.Estado,
                             Motivo_Adopcion = s.Motivo_Adopcion,
                             Condiciones_Hogar = s.Condiciones_Hogar
                         }).ToList();

            return View(lista);
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult RegistrarSolicitud()
        {
            CargarUsuarios();
            CargarAnimalesDisponibles();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult RegistrarSolicitud(SolicitudAdopcionFormVM model)
        {
            if (!ModelState.IsValid)
            {
                CargarUsuarios(model.ID_Usuario);
                CargarAnimalesDisponibles(model.ID_Animal);
                return View(model);
            }

            var animal = db.Animals.FirstOrDefault(a => a.ID_Animal == model.ID_Animal);
            if (animal == null || animal.Estado != "Disponible")
            {
                ModelState.AddModelError("", "El animal seleccionado no está disponible.");
                CargarUsuarios(model.ID_Usuario);
                CargarAnimalesDisponibles(model.ID_Animal);
                return View(model);
            }

            var solicitud = new SolicitudAdopcion
            {
                ID_Usuario = model.ID_Usuario,
                ID_Animal = model.ID_Animal,
                Condiciones_Hogar = model.Condiciones_Hogar,
                Motivo_Adopcion = model.Motivo_Adopcion,
                Otros_Animales = model.Otros_Animales,
                Detalle_Otros_Animales = model.Detalle_Otros_Animales,
                Fecha_Solicitud = DateTime.Now,
                Estado = "Pendiente"
            };

            db.SolicitudAdopcion.Add(solicitud);
            db.SaveChanges();

            TempData["ok"] = "La solicitud de adopción fue registrada correctamente.";
            return RedirectToAction("Solicitudes");
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult Evaluar(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var solicitud = db.SolicitudAdopcion.FirstOrDefault(s => s.ID_Solicitud == id);
            if (solicitud == null)
                return HttpNotFound();

            var usuario = db.Usuarios.FirstOrDefault(u => u.ID_Usuario == solicitud.ID_Usuario);
            var animal = db.Animals.FirstOrDefault(a => a.ID_Animal == solicitud.ID_Animal);

            var vm = new EvaluarSolicitudVM
            {
                ID_Solicitud = solicitud.ID_Solicitud,
                UsuarioNombre = NombreCompletoUsuario(usuario),
                AnimalNombre = animal == null ? "" : animal.Nombre_Animal,
                Motivo_Adopcion = solicitud.Motivo_Adopcion,
                Condiciones_Hogar = solicitud.Condiciones_Hogar,
                Otros_Animales = solicitud.Otros_Animales ?? false,
                Detalle_Otros_Animales = solicitud.Detalle_Otros_Animales,
                Estado = solicitud.Estado
            };

            ViewBag.Estados = new SelectList(
                new List<string> { "Pendiente", "En revisión", "Aprobada", "Rechazada" },
                vm.Estado
            );

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult Evaluar(EvaluarSolicitudVM model)
        {
            var solicitud = db.SolicitudAdopcion.FirstOrDefault(s => s.ID_Solicitud == model.ID_Solicitud);
            if (solicitud == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Estados = new SelectList(
                    new List<string> { "Pendiente", "En revisión", "Aprobada", "Rechazada" },
                    model.Estado
                );
                return View(model);
            }

            solicitud.Estado = model.Estado;
            db.SaveChanges();

            TempData["ok"] = "La solicitud fue evaluada correctamente.";
            return RedirectToAction("Solicitudes");
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult AsignarAnimal(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var solicitud = db.SolicitudAdopcion.FirstOrDefault(s => s.ID_Solicitud == id);
            if (solicitud == null)
                return HttpNotFound();

            if (solicitud.Estado != "Aprobada")
            {
                TempData["error"] = "Solo se puede asignar animal a solicitudes aprobadas.";
                return RedirectToAction("Solicitudes");
            }

            bool yaExiste = db.Adopcion.Any(a => a.ID_Solicitud == solicitud.ID_Solicitud);
            if (yaExiste)
            {
                TempData["error"] = "Esta solicitud ya tiene una adopción registrada.";
                return RedirectToAction("Solicitudes");
            }

            var usuario = db.Usuarios.FirstOrDefault(u => u.ID_Usuario == solicitud.ID_Usuario);
            var animalSolicitud = db.Animals.FirstOrDefault(a => a.ID_Animal == solicitud.ID_Animal);

            CargarAnimalesDisponibles(solicitud.ID_Animal);

            var vm = new AsignarAnimalVM
            {
                ID_Solicitud = solicitud.ID_Solicitud,
                ID_Animal = solicitud.ID_Animal ?? 0,
                Fecha_Adopcion = DateTime.Now,
                Estado_Adopcion = "Activa",
                Seguimiento_Inicial = "Llamada de verificación en 7 días",
                SolicitudInfo = string.Format(
                    "Solicitud #{0} - {1} - Animal solicitado: {2}",
                    solicitud.ID_Solicitud,
                    NombreCompletoUsuario(usuario),
                    animalSolicitud == null ? "" : animalSolicitud.Nombre_Animal
                )
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult AsignarAnimal(AsignarAnimalVM model)
        {
            var solicitud = db.SolicitudAdopcion.FirstOrDefault(s => s.ID_Solicitud == model.ID_Solicitud);
            if (solicitud == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
            {
                CargarAnimalesDisponibles(model.ID_Animal);
                return View(model);
            }

            var animal = db.Animals.FirstOrDefault(a => a.ID_Animal == model.ID_Animal);
            if (animal == null || animal.Estado != "Disponible")
            {
                ModelState.AddModelError("", "El animal seleccionado no está disponible.");
                CargarAnimalesDisponibles(model.ID_Animal);
                return View(model);
            }

            bool yaExiste = db.Adopcion.Any(a => a.ID_Solicitud == model.ID_Solicitud);
            if (yaExiste)
            {
                ModelState.AddModelError("", "La solicitud ya tiene una adopción registrada.");
                CargarAnimalesDisponibles(model.ID_Animal);
                return View(model);
            }

            var adopcion = new Adopcion
            {
                ID_Solicitud = model.ID_Solicitud,
                ID_Animal = model.ID_Animal,
                Fecha_Adopcion = model.Fecha_Adopcion,
                Estado_Adopcion = model.Estado_Adopcion,
                Seguimiento_Inicial = model.Seguimiento_Inicial
            };

            db.Adopcion.Add(adopcion);

            solicitud.Estado = "Adoptada";
            animal.Estado = "Adoptado";

            db.SaveChanges();

            TempData["ok"] = "La adopción fue registrada y el animal quedó asignado correctamente.";
            return RedirectToAction("Index");
        }


        public ActionResult MiPerfil()
        {
            var idUsuario = ObtenerIdUsuarioActual();
            if (idUsuario == null)
            {
                TempData["error"] = "No se encontró el vínculo entre el usuario logueado y la tabla Usuario.";
                return RedirectToAction("Index", "Home");
            }

            var usuario = db.Usuarios.FirstOrDefault(u => u.ID_Usuario == idUsuario.Value);

            var solicitudes = (from s in db.SolicitudAdopcion
                               join u in db.Usuarios on s.ID_Usuario equals u.ID_Usuario
                               join a in db.Animals on s.ID_Animal equals a.ID_Animal into aj
                               from a in aj.DefaultIfEmpty()
                               where s.ID_Usuario == idUsuario.Value
                               orderby s.Fecha_Solicitud descending
                               select new SolicitudResumenVM
                               {
                                   ID_Solicitud = s.ID_Solicitud,
                                   ID_Usuario = s.ID_Usuario,
                                   UsuarioNombre = (u.Nombre + " " + u.Apellido1 + " " + u.Apellido2).Trim(),
                                   ID_Animal = s.ID_Animal,
                                   AnimalNombre = a == null ? "" : a.Nombre_Animal,
                                   Fecha_Solicitud = s.Fecha_Solicitud,
                                   Estado = s.Estado,
                                   Motivo_Adopcion = s.Motivo_Adopcion,
                                   Condiciones_Hogar = s.Condiciones_Hogar
                               }).ToList();

            var adopciones = (from ad in db.Adopcion
                              join s in db.SolicitudAdopcion on ad.ID_Solicitud equals s.ID_Solicitud
                              join u in db.Usuarios on s.ID_Usuario equals u.ID_Usuario
                              join a in db.Animals on ad.ID_Animal equals a.ID_Animal
                              where s.ID_Usuario == idUsuario.Value
                              orderby ad.Fecha_Adopcion descending
                              select new AdopcionResumenVM
                              {
                                  ID_Adopcion = ad.ID_Adopcion,
                                  ID_Solicitud = ad.ID_Solicitud,
                                  AnimalNombre = a.Nombre_Animal,
                                  UsuarioNombre = (u.Nombre + " " + u.Apellido1 + " " + u.Apellido2).Trim(),
                                  Fecha_Adopcion = ad.Fecha_Adopcion,
                                  Estado_Adopcion = ad.Estado_Adopcion,
                                  Seguimiento_Inicial = ad.Seguimiento_Inicial
                              }).ToList();

            var seguimientos = (from seg in db.SeguimientosAdopcion
                                join ad in db.Adopcion on seg.ID_Adopcion equals ad.ID_Adopcion
                                join an in db.Animals on ad.ID_Animal equals an.ID_Animal
                                join so in db.SolicitudAdopcion on ad.ID_Solicitud equals so.ID_Solicitud
                                where so.ID_Usuario == idUsuario.Value
                                orderby seg.Fecha_Seguimiento descending
                                select new SeguimientoResumenVM
                                {
                                    ID_Seguimiento = seg.ID_Seguimiento,
                                    ID_Adopcion = seg.ID_Adopcion,
                                    AnimalNombre = an.Nombre_Animal,
                                    Fecha_Seguimiento = seg.Fecha_Seguimiento,
                                    Tipo_Seguimiento = seg.Tipo_Seguimiento,
                                    Estado_Mascota = seg.Estado_Mascota,
                                    Estado_Hogar = seg.Estado_Hogar,
                                    Comentario = seg.Comentario,
                                    Proxima_Accion = seg.Proxima_Accion
                                }).ToList();

            var devoluciones = (from dev in db.DevolucionesAdopcion
                                join ad in db.Adopcion on dev.ID_Adopcion equals ad.ID_Adopcion
                                join an in db.Animals on ad.ID_Animal equals an.ID_Animal
                                join so in db.SolicitudAdopcion on ad.ID_Solicitud equals so.ID_Solicitud
                                where so.ID_Usuario == idUsuario.Value
                                orderby dev.Fecha_Devolucion descending
                                select new DevolucionResumenVM
                                {
                                    ID_Devolucion = dev.ID_Devolucion,
                                    ID_Adopcion = dev.ID_Adopcion,
                                    AnimalNombre = an.Nombre_Animal,
                                    Fecha_Devolucion = dev.Fecha_Devolucion,
                                    Motivo = dev.Motivo,
                                    Estado_Final_Animal = dev.Estado_Final_Animal
                                }).ToList();

            var vm = new MiPerfilAdopcionVM
            {
                NombreUsuario = NombreCompletoUsuario(usuario),
                Solicitudes = solicitudes,
                Adopciones = adopciones,
                Seguimientos = seguimientos,
                Devoluciones = devoluciones
            };

            return View(vm);
        }

        public ActionResult RegistrarSeguimiento(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var adopcion = db.Adopcion.FirstOrDefault(a => a.ID_Adopcion == id);
            if (adopcion == null)
                return HttpNotFound();

            var solicitud = db.SolicitudAdopcion.FirstOrDefault(s => s.ID_Solicitud == adopcion.ID_Solicitud);
            var animal = db.Animals.FirstOrDefault(a => a.ID_Animal == adopcion.ID_Animal);

            var idUsuarioActual = ObtenerIdUsuarioActual();

            if (!EsAdministrador() && (solicitud == null || solicitud.ID_Usuario != idUsuarioActual))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            var vm = new RegistrarSeguimientoVM
            {
                ID_Adopcion = adopcion.ID_Adopcion,
                AdopcionInfo = string.Format("Adopción #{0} - {1}", adopcion.ID_Adopcion, animal == null ? "" : animal.Nombre_Animal)
            };

            ViewBag.TiposSeguimiento = new SelectList(new List<string>
            {
                "Llamada",
                "Visita",
                "Correo",
                "WhatsApp"
            });

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarSeguimiento(RegistrarSeguimientoVM model)
        {
            var adopcion = db.Adopcion.FirstOrDefault(a => a.ID_Adopcion == model.ID_Adopcion);
            if (adopcion == null)
                return HttpNotFound();

            var solicitud = db.SolicitudAdopcion.FirstOrDefault(s => s.ID_Solicitud == adopcion.ID_Solicitud);
            var idUsuarioActual = ObtenerIdUsuarioActual();

            if (!EsAdministrador() && (solicitud == null || solicitud.ID_Usuario != idUsuarioActual))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TiposSeguimiento = new SelectList(new List<string>
                {
                    "Llamada",
                    "Visita",
                    "Correo",
                    "WhatsApp"
                }, model.Tipo_Seguimiento);

                return View(model);
            }

            var seguimiento = new SeguimientoAdopcion
            {
                ID_Adopcion = model.ID_Adopcion,
                ID_Usuario = idUsuarioActual,
                Fecha_Seguimiento = DateTime.Now,
                Tipo_Seguimiento = model.Tipo_Seguimiento,
                Estado_Mascota = model.Estado_Mascota,
                Estado_Hogar = model.Estado_Hogar,
                Comentario = model.Comentario,
                Proxima_Accion = model.Proxima_Accion
            };

            db.SeguimientosAdopcion.Add(seguimiento);
            db.SaveChanges();

            TempData["ok"] = "El seguimiento post-adopción se registró correctamente.";
            return RedirectToAction(EsAdministrador() ? "Index" : "MiPerfil");
        }

        [Authorize(Roles = "Administrador")]
        public ActionResult RegistrarDevolucion(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var adopcion = db.Adopcion.FirstOrDefault(a => a.ID_Adopcion == id);
            if (adopcion == null)
                return HttpNotFound();

            var animal = db.Animals.FirstOrDefault(a => a.ID_Animal == adopcion.ID_Animal);

            var vm = new RegistrarDevolucionVM
            {
                ID_Adopcion = adopcion.ID_Adopcion,
                Fecha_Devolucion = DateTime.Now,
                Estado_Final_Animal = "Disponible",
                AdopcionInfo = string.Format("Adopción #{0} - {1}", adopcion.ID_Adopcion, animal == null ? "" : animal.Nombre_Animal)
            };

            ViewBag.EstadosFinales = new SelectList(new List<string>
            {
                "Disponible",
                "En evaluación",
                "Tratamiento"
            });

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public ActionResult RegistrarDevolucion(RegistrarDevolucionVM model)
        {
            var adopcion = db.Adopcion.FirstOrDefault(a => a.ID_Adopcion == model.ID_Adopcion);
            if (adopcion == null)
                return HttpNotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.EstadosFinales = new SelectList(new List<string>
                {
                    "Disponible",
                    "En evaluación",
                    "Tratamiento"
                }, model.Estado_Final_Animal);

                return View(model);
            }

            var idUsuarioActual = ObtenerIdUsuarioActual();

            var devolucion = new DevolucionAdopcion
            {
                ID_Adopcion = model.ID_Adopcion,
                ID_Usuario_Registro = idUsuarioActual,
                Fecha_Devolucion = model.Fecha_Devolucion,
                Motivo = model.Motivo,
                Observacion = model.Observacion,
                Estado_Final_Animal = model.Estado_Final_Animal
            };

            db.DevolucionesAdopcion.Add(devolucion);

            adopcion.Estado_Adopcion = "Devuelta";

            var solicitud = db.SolicitudAdopcion.FirstOrDefault(s => s.ID_Solicitud == adopcion.ID_Solicitud);
            if (solicitud != null)
                solicitud.Estado = "Devuelta";

            var animal = db.Animals.FirstOrDefault(a => a.ID_Animal == adopcion.ID_Animal);
            if (animal != null)
                animal.Estado = model.Estado_Final_Animal;

            db.SaveChanges();

            TempData["ok"] = "La devolución/adopción fallida fue registrada correctamente.";
            return RedirectToAction("Index");
        }

        public ActionResult ReporteAdopciones(DateTime? fechaInicio, DateTime? fechaFin, string estado)
        {
            var idUsuarioActual = ObtenerIdUsuarioActual();
            bool esAdmin = EsAdministrador();

            var query = from s in db.SolicitudAdopcion
                        join u in db.Usuarios on s.ID_Usuario equals u.ID_Usuario into uj
                        from u in uj.DefaultIfEmpty()
                        join an in db.Animals on s.ID_Animal equals an.ID_Animal into aj
                        from an in aj.DefaultIfEmpty()
                        join ad in db.Adopcion on s.ID_Solicitud equals ad.ID_Solicitud into adg
                        from ad in adg.DefaultIfEmpty()
                        select new
                        {
                            Solicitud = s,
                            Usuario = u,
                            Animal = an,
                            Adopcion = ad
                        };

            if (!esAdmin && idUsuarioActual.HasValue)
            {
                query = query.Where(x => x.Solicitud.ID_Usuario == idUsuarioActual.Value);
            }

            if (fechaInicio.HasValue)
            {
                var inicio = fechaInicio.Value.Date;
                query = query.Where(x => x.Solicitud.Fecha_Solicitud >= inicio);
            }

            if (fechaFin.HasValue)
            {
                var fin = fechaFin.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.Solicitud.Fecha_Solicitud <= fin);
            }

            if (!string.IsNullOrWhiteSpace(estado))
            {
                query = query.Where(x =>
                    x.Solicitud.Estado == estado ||
                    (x.Adopcion != null && x.Adopcion.Estado_Adopcion == estado));
            }

            var registrosBase = query.ToList();

            var registros = registrosBase.Select(x => new ReporteAdopcionRowVM
            {
                ID_Solicitud = x.Solicitud.ID_Solicitud,
                ID_Adopcion = x.Adopcion == null ? (int?)null : x.Adopcion.ID_Adopcion,
                UsuarioNombre = x.Usuario == null ? "" : (x.Usuario.Nombre + " " + x.Usuario.Apellido1 + " " + x.Usuario.Apellido2).Trim(),
                AnimalNombre = x.Animal == null ? "" : x.Animal.Nombre_Animal,
                Fecha_Solicitud = x.Solicitud.Fecha_Solicitud,
                Fecha_Adopcion = x.Adopcion == null ? (DateTime?)null : x.Adopcion.Fecha_Adopcion,
                EstadoSolicitud = x.Solicitud.Estado,
                EstadoAdopcion = x.Adopcion == null ? "" : x.Adopcion.Estado_Adopcion,
                CantidadSeguimientos = x.Adopcion == null ? 0 : db.SeguimientosAdopcion.Count(sg => sg.ID_Adopcion == x.Adopcion.ID_Adopcion),
                TieneDevolucion = x.Adopcion != null && db.DevolucionesAdopcion.Any(d => d.ID_Adopcion == x.Adopcion.ID_Adopcion)
            }).OrderByDescending(x => x.Fecha_Solicitud).ToList();

            var vm = new ReporteAdopcionesVM
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin,
                Estado = estado,
                EsAdministrador = esAdmin,
                Registros = registros,
                TotalSolicitudes = registros.Count,
                TotalAprobadas = registros.Count(x => x.EstadoSolicitud == "Aprobada" || x.EstadoSolicitud == "Adoptada"),
                TotalAdopcionesActivas = registros.Count(x => x.EstadoAdopcion == "Activa"),
                TotalDevoluciones = registros.Count(x => x.TieneDevolucion)
            };

            return View(vm);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}