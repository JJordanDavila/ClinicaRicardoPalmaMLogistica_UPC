using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models.Entidades
{
    public class Datos
    {
        public string fechaActualizacion { get; set; }
        public string ruc { get; set; }
        public string razonSocial { get; set; }
        public string estadoContribuyente { get; set; }
        public string condicionDomicilio { get; set; }
        public string ubigeo { get; set; }
        public string tipoVia { get; set; }
        public string nombreVia { get; set; }
        public string codigoZona { get; set; }
        public string tipoZona { get; set; }
        public string numero { get; set; }
        public string interior { get; set; }
        public string lote { get; set; }
        public string departamento { get; set; }
        public string manzana { get; set; }
        public string kilometro { get; set; }
        public List<DatosDetalle> locales { get; set; }

        public Datos()
        {
            locales = new List<DatosDetalle>();
        }
    }

    public class DatosDetalle
    {
        public string ubigeo { get; set; }
        public string tipoVia { get; set; }
        public string nombreVia { get; set; }
        public string codigoZona { get; set; }
        public string tipoZona { get; set; }
        public string numero { get; set; }
        public string interior { get; set; }
        public string lote { get; set; }
        public string departamento { get; set; }
        public string manzana { get; set; }
        public string kilometro { get; set; }
    }

    public class ErrorMessage
    {
        public string Estado { get; set; }
        public string Mensaje { get; set; }
    }
}