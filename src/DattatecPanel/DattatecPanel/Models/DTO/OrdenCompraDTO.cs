using System;

namespace DattatecPanel.Models.DTO
{
    public partial class OrdenCompraDTO
    {
        public string CodigoDetalle { get; set; }
        public string NumeroOC { get; set; }
        public string RazonSocial { get; set; }
        public DateTime Fecha { get; set; }
        public string Area { get; set; }
        public string Solicitante { get; set; }
        public string Direccion { get; set; }
        public string Observacion { get; set; }
        public DateTime FechaEntrega { get; set; }
        public int AñoActual { get; set; }
    }
}