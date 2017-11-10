using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{

    [Table("GL_DetallePostulante")]
    public class DetallePostulante
    {
        [Key]
        public int DetalleId { get; set; }
        public int PostulanteId { get; set; }
        //public string NombreArchivo { get; set; }
        // public string Archivo { get; set; }

        public virtual Postulante Postulante { get; set; }
    }
}