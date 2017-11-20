using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models.DTO
{


    public class ArchivosPresentadosDTO
    {
        public int PostulanteId { get; set; }
        public int Id { get; set; }
        public string Nombre { get; set; }
    }

    public class ValidarPostulanteDTO
    {
        //Datos del postulante
        public int PostulanteId { get; set; }
        public string RUC { get; set; }
        [Display(Name = "Razón Social")]
        public string RazonSocial { get; set; }
        public string Correo { get; set; }
        public int RubroID { get; set; }
        public string Rubro { get; set; }
        public List<ArchivosPresentadosDTO> ListaArchivosPresentados { get; set; }

        [Display(Name = "N° Convocatoria")]
        public string NumeroConvocatoria { get; set; }

        //Datos del mantenimiento
        [Display(Name = "¿Presentó Ficha Ruc?")]
        public bool FichaRuc { get; set; }
        [Display(Name = "¿Presentó Carta de Presentación?")]
        public bool CartaPresentacion { get; set; }

        public ValidarPostulanteDTO() {
            ListaArchivosPresentados = new List<ArchivosPresentadosDTO>();
        }

    }
}