using System;

namespace DattatecPanel.Models.DTO
{
    public partial class LicitacionDTO
    {
        public int LicitacionID { get; set; }
        public int RequerimientoCompraID { get; set; }
        public string Numero { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaConvocatoria { get; set; }
        public DateTime FecRecepcionConsultas { get; set; }
        public DateTime FecAbsolucionConsultas { get; set; }
        public DateTime FecRecepcionExpediente { get; set; }
        public DateTime FecEvaluacionIni { get; set; }
        public DateTime FecEvaluacionFin { get; set; }
        public DateTime FecAdjudicacion { get; set; }

    }
}