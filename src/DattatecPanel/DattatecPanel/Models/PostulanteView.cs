using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    public class PostulanteView
    {
        public Convocatoria Convocatoria { set; get; }
        public Postulante Postulante { set; get; }
        public List<DetallePostulante> DetallePostulantes { set; get; }
        public DetalleConvocatoria DetalleConvocatoria { set; get; }

    }
}