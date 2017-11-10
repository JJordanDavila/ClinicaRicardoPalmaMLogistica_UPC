using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    [Table("GL_Postulante")]
    public class Postulante
    {
        [Key]
        public int PostulanteId { get; set; }
        public string RazonSocial { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public string RUC { get; set; }
        public bool ConstanciaRNP { get; set; }
        //public DateTime Fecha_Registro { get; set; }

        public ICollection<DetallePostulante> DetallePostulantes { set; get; }
    
    }
}