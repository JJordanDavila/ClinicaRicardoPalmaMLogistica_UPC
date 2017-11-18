using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DattatecPanel.Models.Entidades
{
    [Table("GL_DetallePostulante")]
    public class DetallePostulante
    {
        [Key]
        public int DetalleId { get; set; }
        public int PostulanteId { get; set; }

        public string NombreArchivo { get; set; }
        public byte[] Archivo { get; set; }

        public virtual Postulante Postulante { get; set; }
    }
}