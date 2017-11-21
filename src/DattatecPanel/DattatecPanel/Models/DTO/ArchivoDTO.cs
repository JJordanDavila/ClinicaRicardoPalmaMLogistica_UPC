using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models.DTO
{
    public class ArchivoDTO
    {
        public int Id { get; set; }
        public byte[] Datos { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
    }
}