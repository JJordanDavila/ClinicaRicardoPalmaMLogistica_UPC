using System;
using System.Web;

namespace DattatecPanel.Models.DTO
{
    public class ConvocatoriaDTO
    {
        public int Convocatoriaid { get; set; }
        public string Numero { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int RubroID { get; set; }
        public int EmpleadoID { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaSuspension { get; set; }
        public string ObservacionSuspension { get; set; }
        public HttpPostedFileBase Requisito { get; set; }
    }
}