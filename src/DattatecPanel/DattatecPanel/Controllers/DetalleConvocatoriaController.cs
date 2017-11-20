using DattatecPanel.Context;
using DattatecPanel.Models;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Entidades;
using DattatecPanel.Models.Util;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace DattatecPanel.Controllers
{
    public class DetalleConvocatoriaController : Controller
    {
        private MailSMTP correo = new MailSMTP();
        //
        // GET: /DetalleConvocatoria/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarDetalleConvocatoriaPostulante(string numeroConvocatoria, string ruc, string razonSocial)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(numeroConvocatoria)) numeroConvocatoria = numeroConvocatoria.Trim();
                if (!string.IsNullOrWhiteSpace(ruc)) ruc = ruc.Trim();
                if (!string.IsNullOrWhiteSpace(razonSocial)) razonSocial = razonSocial.Trim();
                var lista = new DetalleConvocatoriaModel().ListarDetalleConvocatoriaPostulante(numeroConvocatoria, ruc, razonSocial);
                var jsonresult = Json(new { rows = lista }, JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = int.MaxValue;
                return jsonresult;
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public FileResult DescargarArchivo(int? postulanteId, int? detalleId)
        {
            try
            {
                var archivo = new DetalleConvocatoriaModel().DescargarArchivo(postulanteId, detalleId);
                return File(archivo.Datos, archivo.Tipo, archivo.Nombre);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ActionResult Validar(int? convocatoriaId, int? postulanteId)
        {
            try {
                var datosPostulante = new DetalleConvocatoriaModel().ObtenerDatosPostulante(convocatoriaId, postulanteId);
                return View("Validar", datosPostulante);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Validar(ValidarPostulanteDTO datos)
        {
            try
            {
                RespuestaJsonDTO response = null;
                if (!datos.FichaRuc || !datos.CartaPresentacion)
                {
                    response = new RespuestaJsonDTO() { codigo = HttpStatusCode.OK, mensaje = "", mensajeInfo = "Es requisito que todos los postulantes presenten su ficha RUC y su carta de presentación" };   
                }
                else {
                    response = new DetalleConvocatoriaModel().ValidarPostulante(datos);
                    if (string.IsNullOrWhiteSpace(response.mensajeInfo)) {
                        StringBuilder body = new StringBuilder();
                        body.Append("Estimado " + datos.RazonSocial + ":");
                        body.Append("<br></br>");
                        body.Append("<br></br>");
                        body.Append("Su solicitud para ser proveedor de la Clínica Ricardo Palma ha sido aprobada.");
                        correo.EnviarCorreo("Clinica Ricardo Palma", datos.Correo, "Aprobación de Solicitud Convocatoria N° " + datos.NumeroConvocatoria, body.ToString(), true, null);
                    }
                }
                return Json(new{
                    statusCode = response.codigo,
                    mensaje = response.mensaje,
                    mensajeInfo = response.mensajeInfo
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Rechazar(int? convocatoriaId, int? postulanteId)
        {
            try
            {
                var datosPostulante = new DetalleConvocatoriaModel().ObtenerDatosRechazar(convocatoriaId, postulanteId);
                return View("Rechazar", datosPostulante);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult Rechazar(RechazarPostulanteDTO datos)
        {
            try
            {
                RespuestaJsonDTO response = null;
                if (string.IsNullOrWhiteSpace(datos.Comentario))
                {
                    response = new RespuestaJsonDTO() { codigo = HttpStatusCode.OK, mensaje = "", mensajeInfo = "Es requisito ingresar un comentario" };
                }
                else {
                    response = new DetalleConvocatoriaModel().RechazarPostulante(datos);
                    if (string.IsNullOrWhiteSpace(response.mensajeInfo))
                    {
                        StringBuilder body = new StringBuilder();
                        body.Append("Estimado " + datos.RazonSocial + ":");
                        body.Append("<br></br>");
                        body.Append("<br></br>");
                        body.Append("Su solicitud para ser proveedor de la Clínica Ricardo Palma ha sido rechazada.");
                        body.Append("<br></br>");
                        body.Append("<br></br>");
                        body.Append("Comentario:");
                        body.Append("<br></br>");
                        body.Append("<br></br>");
                        body.Append(datos.Comentario);
                        correo.EnviarCorreo("Clinica Ricardo Palma", datos.Correo, "Rechazo de Solicitud Convocatoria N° " + datos.NumeroConvocatoria, body.ToString(), true, null);
                    }
                }
                return Json(new
                {
                    statusCode = response.codigo,
                    mensaje = response.mensaje,
                    mensajeInfo = response.mensajeInfo
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

	}
}