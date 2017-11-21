using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models.DTO
{
    public class PostulanteConvocatoriaDTO
    {
        public int ConvocatoriaId { get; set; }
        public int PostulanteId { get; set; }
        public string Numero { get; set; }
        public string RUC { get; set; }
        public string RazonSocial { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha_Registro { get; set; }
    }
}