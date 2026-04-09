using System;

namespace AdoptameLiberia.Models.Mascotas
{
    public class HistorialMedicoModel
    {
        public int ID_Historial { get; set; }
        public int ID_Animal { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public string Detalle { get; set; }
    }
}
