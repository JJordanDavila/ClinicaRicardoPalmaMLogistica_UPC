using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models.DTO
{
    public class RechazarPostulanteDTO
    {
        //Datos del postulante
        public int PostulanteId { get; set; }
        public string RUC { get; set; }
        [Display(Name = "Razón Social")]
        public string RazonSocial { get; set; }
        public string Correo { get; set; }
        public int RubroID { get; set; }
        public string Rubro { get; set; }
        
        [Display(Name = "N° Convocatoria")]
        public string NumeroConvocatoria { get; set; }

        //Datos del mantenimiento
        [DataType(DataType.MultilineText)]
        [Display(Name = "Comentario")]
        public string Comentario { get; set; }

        public int ConvocatoriaId { get; set; }
    }
}