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
        [Column(Order = 0), Key]
        public int DetalleId { get; set; }
        [Column(Order = 1), Key]
        public int PostulanteId { get; set; }

        [Column(Order = 2)]
        public string NombreArchivo { get; set; }
        [Column(Order = 3)]
        public byte[] Archivo { get; set; }

        public virtual Postulante Postulante { get; set; }
    }
}