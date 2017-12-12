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
            Session["listaAdjuntos"] = null;
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

        [HttpPost]
        public FileResult DescargarArchivoSesion(int archivoID)
        {
            try
            {
                ArchivoDTO archivo = ((List<ArchivoDTO>)Session["listaAdjuntos"]).Find(x => x.Id == archivoID);
                return File(archivo.Datos, archivo.Tipo, archivo.Nombre);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public ActionResult EliminarArchivoSesion(int archivoIDE, int convocatoriaIDE)
        {
            try
            {
                var postulante = new PostulanteModel().MostrarDatosVistaRegistrar(convocatoriaIDE);
                List<ArchivoDTO> lista = (List<ArchivoDTO>)Session["listaAdjuntos"];
                ArchivoDTO archivo = lista.Find(x => x.Id == archivoIDE);
                lista.Remove(archivo);
                postulante.ListaAdjuntos = lista;
                return View("RegistrarPostulante", postulante);
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
                    entidadPostulanteDTO.ListaAdjuntos = (List<ArchivoDTO>)Session["listaAdjuntos"];
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
        public ActionResult Cargar(HttpPostedFileBase postedFile, int convocatoriaIDU)
        {
            int id;
            var postulante = new PostulanteModel().MostrarDatosVistaRegistrar(convocatoriaIDU);
            

            if (!postedFile.FileName.EndsWith("pdf"))
            {
                ViewBag.ValidarMensaje = true;
                ViewBag.MostrarMensajeArchivo = "Solo se permite subir archivos en formato PDF";
                return View("RegistrarPostulante", postulante);
            }

            if (postedFile.ContentLength > 5000000)
            {
                ViewBag.ValidarMensaje = true;
                ViewBag.MostrarMensajeArchivo = "El archivo no debe exceder de 5 MB";
                return View("RegistrarPostulante", postulante);
            }

            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }
            List<ArchivoDTO> lista;
            if (Session["listaAdjuntos"] == null)
                lista = new List<ArchivoDTO>();
            else
                lista = (List<ArchivoDTO>)Session["listaAdjuntos"];
            if (lista.Count == 0)
                id = 1;
            else
                id = (lista[lista.Count - 1].Id + 1);
            lista.Add(new ArchivoDTO() { Id = id, Datos = bytes, Nombre = postedFile.FileName, Tipo = "application/pdf" });
            Session["listaAdjuntos"] = lista;
            postulante.ListaAdjuntos = lista;
            return View("RegistrarPostulante", postulante);
        }

        public ActionResult ListarConvocatorias(int page, int pageSize)
        {
            try
            {
                var lista = new PostulanteModel().ListarConvocatorias(page, pageSize);
                var jsonresult = Json(new { rows = lista.lista, total = lista.total }, JsonRequestBehavior.AllowGet);
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