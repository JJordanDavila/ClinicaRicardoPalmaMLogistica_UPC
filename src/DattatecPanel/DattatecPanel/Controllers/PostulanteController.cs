using DattatecPanel.Context;
using DattatecPanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Entidades;
using System.IO;
using System.Net;

namespace DattatecPanel.Controllers
{
    public class PostulanteController : Controller
    {
        // GET: Postulante
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public FileResult DescargarArchivo(int convocatoriaID)
        {
            try
            {
                var archivo = new PostulanteModel().DescargarArchivo(convocatoriaID);
                return File(archivo.Datos, archivo.Tipo, archivo.Nombre);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ActionResult RegistrarPostulante(int id)
        {
            var postulante = new PostulanteModel().MostrarDatosVistaRegistrar(id);
            return View("RegistrarPostulante", postulante);
        }

        [HttpPost]
        public ActionResult RegistrarPostulante(PostulanteDTO entidadPostulanteDTO)
        {
            try
            {
                var response = new PostulanteModel().ValidarRuc(entidadPostulanteDTO.RUC, entidadPostulanteDTO.IdConvocatoria);

                if (response.mensaje.Equals("1"))
                {
                    response = new PostulanteModel().GuardarPostulante(entidadPostulanteDTO);
                }


                return Json(new { statusCode = HttpStatusCode.OK, mensaje = response.mensaje, mensajeInfo = response.mensajeInfo },
                   JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);

            }
        }


        public ActionResult VerificarRUC(string numeroRUC)
        {
            try
            {
                var response = new PostulanteModel().VerificarRUC(numeroRUC);

                return Json(new { statusCode = HttpStatusCode.OK, mensaje = response.mensaje, mensajeInfo = response.mensajeInfo, mensajeDireccion = response.mensajeDireccion },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult Cargar(HttpPostedFileBase archivo)
        {
            var productId = ViewBag.ConvocatoriaId;
            DetallePostulante detallepostulante;
            var entidadPostulante = Session["entidadPostulante"] as Postulante;
            var s_detallePostulante = Session["detallePostulante"] as DetallePostulante;

            if (!archivo.FileName.EndsWith("pdf"))
            {
                ViewBag.MessageAdvEditLC = true;
                ViewBag.MessageAdvertenciaLC = "Solo adjuntar archivo en formato PDF.";
                return View("RegistrarPostulante", entidadPostulante);
            }

            byte[] data = null;
            if (archivo != null)
            {
                using (Stream inputStream = archivo.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    data = memoryStream.ToArray();
                }
            }

            detallepostulante = new DetallePostulante
            {
                NombreArchivo = archivo.FileName.ToString(),
                Archivo = data
            };

            Session["detallePostulante"] = detallepostulante;
            entidadPostulante.DetallePostulantes.Add(detallepostulante);

            return View("RegistrarPostulante", entidadPostulante);
        }

        public ActionResult ListarConvocatorias()
        {
            try
            {
                var lista = new PostulanteModel().ListarConvocatorias();
                var jsonresult = Json(new { rows = lista }, JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = int.MaxValue;
                return jsonresult;
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}