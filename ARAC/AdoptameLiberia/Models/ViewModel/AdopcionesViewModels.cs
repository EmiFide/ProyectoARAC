using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdoptameLiberia.Models.ViewModel
{
    public class SolicitudAdopcionFormVM
    {
        [Required]
        [Display(Name = "Usuario adoptante")]
        public int ID_Usuario { get; set; }

        [Required]
        [Display(Name = "Animal solicitado")]
        public int ID_Animal { get; set; }

        [Required]
        [StringLength(300)]
        [Display(Name = "Condiciones del hogar")]
        public string Condiciones_Hogar { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Motivo de adopción")]
        public string Motivo_Adopcion { get; set; }

        [Display(Name = "¿Tiene otros animales?")]
        public bool Otros_Animales { get; set; }

        [StringLength(200)]
        [Display(Name = "Detalle de otros animales")]
        public string Detalle_Otros_Animales { get; set; }
    }

    public class SolicitudResumenVM
    {
        public int ID_Solicitud { get; set; }
        public int? ID_Usuario { get; set; }
        public string UsuarioNombre { get; set; }
        public int? ID_Animal { get; set; }
        public string AnimalNombre { get; set; }
        public DateTime? Fecha_Solicitud { get; set; }
        public string Estado { get; set; }
        public string Motivo_Adopcion { get; set; }
        public string Condiciones_Hogar { get; set; }
    }

    public class EvaluarSolicitudVM
    {
        public int ID_Solicitud { get; set; }
        public string UsuarioNombre { get; set; }
        public string AnimalNombre { get; set; }
        public string Motivo_Adopcion { get; set; }
        public string Condiciones_Hogar { get; set; }
        public bool Otros_Animales { get; set; }
        public string Detalle_Otros_Animales { get; set; }

        [Required]
        [Display(Name = "Nuevo estado")]
        public string Estado { get; set; }
    }

    public class AsignarAnimalVM
    {
        [Required]
        public int ID_Solicitud { get; set; }

        [Required]
        [Display(Name = "Animal a asignar")]
        public int ID_Animal { get; set; }

        [Display(Name = "Fecha de adopción")]
        public DateTime Fecha_Adopcion { get; set; }

        [Required]
        [Display(Name = "Estado de la adopción")]
        public string Estado_Adopcion { get; set; }

        [StringLength(300)]
        [Display(Name = "Seguimiento inicial")]
        public string Seguimiento_Inicial { get; set; }

        public string SolicitudInfo { get; set; }
    }

    public class RegistrarSeguimientoVM
    {
        [Required]
        public int ID_Adopcion { get; set; }

        public string AdopcionInfo { get; set; }

        [Required]
        [Display(Name = "Tipo de seguimiento")]
        public string Tipo_Seguimiento { get; set; }

        [Display(Name = "Estado de la mascota")]
        public string Estado_Mascota { get; set; }

        [Display(Name = "Estado del hogar")]
        public string Estado_Hogar { get; set; }

        [Required]
        public string Comentario { get; set; }

        [Display(Name = "Próxima acción")]
        public string Proxima_Accion { get; set; }
    }

    public class RegistrarDevolucionVM
    {
        [Required]
        public int ID_Adopcion { get; set; }

        public string AdopcionInfo { get; set; }

        [Display(Name = "Fecha de devolución")]
        public DateTime Fecha_Devolucion { get; set; }

        [Required]
        public string Motivo { get; set; }

        public string Observacion { get; set; }

        [Required]
        [Display(Name = "Estado final del animal")]
        public string Estado_Final_Animal { get; set; }
    }

    public class AdopcionResumenVM
    {
        public int ID_Adopcion { get; set; }
        public int ID_Solicitud { get; set; }
        public string AnimalNombre { get; set; }
        public string UsuarioNombre { get; set; }
        public DateTime Fecha_Adopcion { get; set; }
        public string Estado_Adopcion { get; set; }
        public string Seguimiento_Inicial { get; set; }
    }

    public class SeguimientoResumenVM
    {
        public int ID_Seguimiento { get; set; }
        public int ID_Adopcion { get; set; }
        public string AnimalNombre { get; set; }
        public DateTime Fecha_Seguimiento { get; set; }
        public string Tipo_Seguimiento { get; set; }
        public string Estado_Mascota { get; set; }
        public string Estado_Hogar { get; set; }
        public string Comentario { get; set; }
        public string Proxima_Accion { get; set; }
    }

    public class DevolucionResumenVM
    {
        public int ID_Devolucion { get; set; }
        public int ID_Adopcion { get; set; }
        public string AnimalNombre { get; set; }
        public DateTime Fecha_Devolucion { get; set; }
        public string Motivo { get; set; }
        public string Estado_Final_Animal { get; set; }
    }

    public class MiPerfilAdopcionVM
    {
        public string NombreUsuario { get; set; }
        public List<SolicitudResumenVM> Solicitudes { get; set; }
        public List<AdopcionResumenVM> Adopciones { get; set; }
        public List<SeguimientoResumenVM> Seguimientos { get; set; }
        public List<DevolucionResumenVM> Devoluciones { get; set; }
    }

    public class ReporteAdopcionRowVM
    {
        public int ID_Solicitud { get; set; }
        public int? ID_Adopcion { get; set; }
        public string UsuarioNombre { get; set; }
        public string AnimalNombre { get; set; }
        public DateTime? Fecha_Solicitud { get; set; }
        public DateTime? Fecha_Adopcion { get; set; }
        public string EstadoSolicitud { get; set; }
        public string EstadoAdopcion { get; set; }
        public int CantidadSeguimientos { get; set; }
        public bool TieneDevolucion { get; set; }
    }

    public class ReporteAdopcionesVM
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; }
        public bool EsAdministrador { get; set; }
        public List<ReporteAdopcionRowVM> Registros { get; set; }

        public int TotalSolicitudes { get; set; }
        public int TotalAprobadas { get; set; }
        public int TotalAdopcionesActivas { get; set; }
        public int TotalDevoluciones { get; set; }
    }
}