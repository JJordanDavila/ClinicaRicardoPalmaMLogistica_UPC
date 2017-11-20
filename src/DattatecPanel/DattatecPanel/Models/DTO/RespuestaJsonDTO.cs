using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace DattatecPanel.Models.DTO
{
    public class RespuestaJsonDTO
    {
        public HttpStatusCode codigo { get; set; }
        public string mensaje { get; set; }
        public string mensajeInfo { get; set; }
    }
}