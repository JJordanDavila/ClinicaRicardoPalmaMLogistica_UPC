using DattatecPanel.Context;
using DattatecPanel.Models;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DattatecPanel.Controllers
{
    public class ConvocatoriaController : Controller
    {
        private ClinicaDBContext db = new ClinicaDBContext();
        private MailSMTP correo = new MailSMTP();
        // GET: Convocatoria
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarConvocatoriaProveedores(string numero, string fini, string ffin)
        {
            var dfini = string.IsNullOrEmpty(fini) ? DateTime.MinValue : Convert.ToDateTime(fini);
            var dffin = string.IsNullOrEmpty(ffin) ? DateTime.MaxValue : Convert.ToDateTime(ffin);
            var lista = db.DB_Convocatoria.Where(x => x.Numero.Contains(numero)
           && x.FechaInicio >= dfini
           && x.FechaFin <= dffin
           && x.Estado == "E").ToList().Select(s => new {
               s.Convocatoriaid,
               s.Numero,
               s.FechaInicio,
               s.FechaFin,
               s.Requisito,
               s.Estado,
               s.Rubro.Descripcion,
               s.Empleado.NombreCompleto
           }).ToList();
            var jsonresult =  Json(new { rows = lista }, JsonRequestBehavior.AllowGet);
            jsonresult.MaxJsonLength = int.MaxValue;
            return jsonresult;
        }

        public ActionResult Nuevo()
        {
            Random r = new Random();
            var numero = db.DB_Convocatoria.OrderByDescending(x => x.Numero).First().Numero;
            var correlativo = Convert.ToInt32(numero.ToString().Substring(6)) + 1;
            ViewBag.NuevoNumeroConvocatoria = numero.Substring(0, 6) + correlativo.ToString();
            CargarCombos();
            return View();
        }

        [HttpPost]
        public ActionResult Nuevo(ConvocatoriaDTO entidad)
        {
            try
            {
                var mensaje = string.Empty;
                byte[] data = null;
                if (entidad.RequisitoFile != null)
                {
                    using (Stream inputStream = entidad.RequisitoFile.InputStream)
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

                entidad.Estado = "E";
                Convocatoria convocatoria = new Convocatoria
                {
                    Convocatoriaid = entidad.Convocatoriaid,
                    Numero = entidad.Numero,
                    FechaInicio = entidad.FechaInicio,
                    FechaFin = entidad.FechaFin,
                    Estado = entidad.Estado,
                    RubroID = entidad.RubroID,
                    EmpleadoID = entidad.EmpleadoID,
                    Requisito = data == null ? entidad.Requisito : data
                };
                if (entidad.Convocatoriaid <= 0)
                {
                    var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == convocatoria.EmpleadoID).FirstOrDefault();
                    var cuerpoCorreo = "Se registro la convocatoria con el numero : " + convocatoria.Numero.ToString();
                    db.DB_Convocatoria.Add(convocatoria);
                    db.SaveChanges();
                    correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Creación de convocatoria", cuerpoCorreo, false, null);
                    mensaje = "Se registro con exito";
                }
                else
                {
                    var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == convocatoria.EmpleadoID).FirstOrDefault();
                    var cuerpoCorreo = "Se actualizo la convocatoria con el numero : " + convocatoria.Numero.ToString();
                    db.Entry(convocatoria).State = EntityState.Modified;
                    db.SaveChanges();
                    correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Actualizacion de convocatoria", cuerpoCorreo, false, null);
                    mensaje = "Se actualizo con exito";
                }
                return Json(new { statusCode = HttpStatusCode.OK, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Actualizar(int id)
        {
            var entidad = db.DB_Convocatoria.Where(x => x.Convocatoriaid == id).First();
            CargarCombos();
            return View("Nuevo", entidad);
        }

        public ActionResult Suspender(int id)
        {
            var entidad = db.DB_Convocatoria.Where(x => x.Convocatoriaid == id).First();
            CargarCombos();
            return View("Suspender", entidad);
        }

        [HttpPost]
        public ActionResult Suspender(Convocatoria entidad)
        {
            try
            {
                var mensaje = string.Empty;
                if (string.IsNullOrEmpty(entidad.ObservacionSuspension))
                {
                    return Json(new { statusCode = HttpStatusCode.OK, mensaje = "Ingrese una observación." }, JsonRequestBehavior.AllowGet);
                }
                var cuerpoCorreo = "Se suspendio la convocatoria con el numero : " + entidad.Numero.ToString();
                var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == entidad.EmpleadoID).FirstOrDefault();
                entidad.Estado = "S";
                entidad.FechaSuspension = DateTime.Now;
                db.Entry(entidad).State = EntityState.Modified;
                db.SaveChanges();
                correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Suspension de convocatoria", cuerpoCorreo, false, null);
                mensaje = "Se suspendio con exito";
                return Json(new { statusCode = HttpStatusCode.OK, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        private void CargarCombos()
        {
            ViewBag.Rubros = db.Rubroes.ToList();
            ViewBag.Solicitantes = db.DB_Empleado.ToList();
        }
    }
}