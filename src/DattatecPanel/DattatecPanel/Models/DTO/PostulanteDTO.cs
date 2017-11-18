using DattatecPanel.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models.DTO
{
    public class PostulanteDTO
    {
        public int PostulanteId { get; set; }
        public string RazonSocial { get; set; }
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public string RUC { get; set; }
        public bool ConstanciaRNP { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public ICollection<DetallePostulante> DetallePostulantes { set; get; }
    }
}