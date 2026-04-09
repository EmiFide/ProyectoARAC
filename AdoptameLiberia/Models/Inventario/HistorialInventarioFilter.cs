using System;

namespace AdoptameLiberia.Models.Inventario
{
    public class HistorialInventarioFilter
    {
        public string UsuarioEmail { get; set; }
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
    }
}
