using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models.DTO
{
    public class ParametroDTO
    {       
        public int ParametroId { get; set; }       
        public DateTime FecIni { get; set; }
        public DateTime FecFin { get; set; }
        public int Intervalo { get; set; }
        public string UnidadMedidaIntervalo { get; set; }

        ////public virtual UnidadMedida UnidadMedidaIntervalo { get; set; }
        public DateTime FecUltPro { get; set; }
        public string UrlServicio01 { get; set; }
        public string UrlServicio02 { get; set; }
        public string EstadoServicioSUNAT { get; set; }
        public string EstadoServicioOSCE { get; set; }
    }
}