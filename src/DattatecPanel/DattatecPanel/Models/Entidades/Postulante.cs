using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DattatecPanel.Models.Entidades
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

        public ICollection<DetallePostulante> DetallePostulantes { get; set; }

        [NotMapped]
        public Convocatoria Convocatoria { set; get; }
    }
}