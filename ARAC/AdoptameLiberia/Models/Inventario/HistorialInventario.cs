using System;
using System.Collections.Generic;

namespace AdoptameLiberia.Models.Inventario
{
    public class HistorialInventario
    {
        public HistorialInventarioFilter Filter { get; set; }
        public List<HistorialInventarioRowVM> Movimientos { get; set; }
    }

    public class HistorialInventarioRowVM
    {
        public DateTime Fecha { get; set; }
        public string Item { get; set; }
        public string Tipo { get; set; }
        public int Cantidad { get; set; }
        public int? StockAnterior { get; set; }
        public int? StockNuevo { get; set; }
        public string Destinatario { get; set; }
        public string Motivo { get; set; }
        public string UsuarioEmail { get; set; }
    }
}
