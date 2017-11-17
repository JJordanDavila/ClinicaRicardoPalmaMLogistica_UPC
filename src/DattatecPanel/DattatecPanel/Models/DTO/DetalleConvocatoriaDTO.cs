using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DattatecPanel.Models.Entidades;

namespace DattatecPanel.Models.DTO
{
    public class DetalleConvocatoriaDTO
    {
        public int ConvocatoriaId { get; set; }       
        public int PostulanteId { get; set; }        
        public virtual Postulante Postulante { get; set; }       
        public DateTime? Fecha_Registro { get; set; }
        public virtual Convocatoria Convocatoria { get; set; }
        public string Correo { get; set; }  

    }
}