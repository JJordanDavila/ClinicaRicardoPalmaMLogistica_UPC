using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace DattatecPanel.Models
{
    public class DetalleConvocatoria
    {
        [Key]
        public int ConvocatoriaId { get; set; }

        public int PostulanteId { get; set; }

        public DateTime Fecha_Registro { get; set; }

        public virtual Postulante Postulante { get; set; }
    }
}