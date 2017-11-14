using DattatecPanel.Models.Entidades;
using System.Collections.Generic;

namespace DattatecPanel.Models
{
    public class PostulanteView
    {
        public ConvocatoriaModel Convocatoria { set; get; }
        public Postulante Postulante { set; get; }
        public List<DetallePostulante> DetallePostulantes { set; get; }
        public DetalleConvocatoria DetalleConvocatoria { set; get; }

    }
}