using DattatecPanel.Models.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models.DTO
{
    public class PostulanteDTO
    {
        [Display(Name ="Número")]
        public string NumeroConvocatoria { get; set; }
        [Display(Name = "RUC")]
        public string RUC { get; set; }
        [Display(Name = "Rubro")]
        public int idRubro { get; set; }
        [Display(Name = "Razón Social")]
        public string RazonSocial { get; set; }
        [Display(Name = "Correo Electronico")]
        public string Correo { get; set; }
        [Display(Name = "Tiene constancia RNP")]
        public bool flagConstanciaRNP { get; set; }
    }
}