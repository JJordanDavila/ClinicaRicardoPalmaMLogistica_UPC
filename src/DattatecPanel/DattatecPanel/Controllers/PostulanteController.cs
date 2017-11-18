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

        public ActionResult RegistrarPostulante(int id)
        {
            var postulante = new PostulanteModel().MostrarDatosVistaRegistrar(id);
            return View("RegistrarPostulante", postulante);
        }

        [HttpPost]
        public ActionResult RegistrarPostulante(Postulante entidadPostulanteDTO)
        {
            try
            {
                var entidadPostulante = Session["entidadPostulante"] as Postulante;
                var s_detallePostulante = Session["detallePostulante"] as DetallePostulante;

                /*  if (entidadPostulanteDTO.RUC.Equals(null))
                  {
                      ViewBag.MessageAdvCreateLC = true;
                      ViewBag.MessageAdvertenciaLC = "Debe ingresar el número RUC";
                      return View(entidadPostulante);
                  }

                  if (entidadPostulanteDTO.RazonSocial.Equals(null))
                  {
                      ViewBag.MessageAdvCreateLC = true;
                      ViewBag.MessageAdvertenciaLC = "Debe ingresar la Raz{on Social";
                      return View(entidadPostulante);
                  }

                  if (entidadPostulanteDTO.Direccion.Equals(null))
                  {
                      ViewBag.MessageAdvCreateLC = true;
                      ViewBag.MessageAdvertenciaLC = "Debe ingresar la dirección";
                      return View(entidadPostulante);
                  }

                  if (entidadPostulanteDTO.Correo.Equals(null))
                  {
                      ViewBag.MessageAdvCreateLC = true;
                      ViewBag.MessageAdvertenciaLC = "Debe ingresar un correo electrónico válido";
                      return View(entidadPostulante);
                  }*/


                entidadPostulante.RUC = entidadPostulanteDTO.RUC;
                entidadPostulante.RazonSocial = entidadPostulanteDTO.RazonSocial;
                entidadPostulante.Direccion = entidadPostulanteDTO.Direccion;
                entidadPostulante.Correo = entidadPostulanteDTO.Correo;
                entidadPostulante.ConstanciaRNP = entidadPostulanteDTO.ConstanciaRNP;

                var response = new PostulanteModel().GuardarPostulante(entidadPostulante, ViewBag.ConvocatoriaId);

                ViewBag.MessageAdvCreateLC = true;
                ViewBag.MessageAdvertenciaLC = response.mensaje;


                return Json(new { statusCode = HttpStatusCode.OK, mensaje = response.mensaje, mensajeInfo = response.mensajeInfo },
                   JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);

                ViewBag.MessageAdvCreateLC = true;
                ViewBag.MessageAdvertenciaLC = ex.Message;
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