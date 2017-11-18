using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models.DTO
{
    public class DetallePostulanteDTO
    {
        public int DetalleId { get; set; }
        public int PostulanteId { get; set; }

        public string NombreArchivo { get; set; }
        public byte[] Archivo { get; set; }

        public virtual PostulanteDTO PostulanteDTO { get; set; }
    }
}