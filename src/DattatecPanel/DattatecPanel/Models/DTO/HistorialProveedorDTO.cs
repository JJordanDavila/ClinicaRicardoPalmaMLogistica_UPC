using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models.DTO
{

    public class ParticipacionLicitacionDTO{
        [Display(Name = "Número")]
        public string Numero { get; set; }
        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [Display(Name = "Fecha de Colocación de Expediente")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaColocacionExpediente { get; set; }
        public string Moneda { get; set; }
        public decimal Monto { get; set; }
    }

    public class ParticipacionCotizacionDTO{
        [Display(Name = "Número")]
        public string Numero { get; set; }
        [Display(Name = "Fecha")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Fecha { get; set; }
        [Display(Name = "Forma de Pago")]
        public string FormaPago { get; set; }
        [Display(Name = "N° días")]
        public int NroDiasPago { get; set; }
    }

    public class HistorialProveedorDTO
    {
        [Display(Name = "R.U.C.")]
        public string Ruc {get;set;}
        [Display(Name = "Razón Social")]
        public string RazonSocial {get;set;}
        [Display(Name = "Estado")]
        public string Estado { get; set; }
        [Display(Name = "Motivo Suspensión")]
        public string ObservacionesSuspension { get; set; }
        [Display(Name = "Fecha Suspensión")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FechaSuspension { get; set; }
        
        public List<ParticipacionLicitacionDTO> ListaParticipacionLicitacionDTO { get; set; }
        public List<ParticipacionCotizacionDTO> ListaParticipacionCotizacionDTO { get; set; }

        public HistorialProveedorDTO()
        {
            this.ListaParticipacionLicitacionDTO = new List<ParticipacionLicitacionDTO>();
            this.ListaParticipacionCotizacionDTO = new List<ParticipacionCotizacionDTO>();
        }
    }
}