using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models.DTO
{
    public class RequestEvaluarProveedor
    {
        public string RUC { get; set; }
        public string RazonSocial { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public int idProveedor { get; set; }
        public string Estado { get; set; }
        public string Observacion { get; set; }
    }
}