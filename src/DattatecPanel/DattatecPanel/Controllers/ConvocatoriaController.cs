using DattatecPanel.Context;
using DattatecPanel.Models;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            var dffin = string.IsNullOrEmpty(ffin) ? DateTime.Now : Convert.ToDateTime(ffin);
            var lista = db.DB_Convocatoria.Where(x => x.Numero.Contains(numero)
           && x.FechaInicio >= dfini
           && x.FechaFin <= dffin).ToList().Select(s => new {
               s.Convocatoriaid,
               s.Numero,
               s.FechaInicio,
               s.FechaFin,
               s.Requisito,
               s.Estado,
               s.Rubro.Descripcion,
               s.Empleado.NombreCompleto
           }).ToList();
            return Json(new { rows = lista }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Nuevo()
        {
            Random r = new Random();
            var numero = db.DB_Convocatoria.OrderByDescending(x => x.Numero).First().Numero;
            ViewBag.NuevoNumeroConvocatoria = Convert.ToInt32(numero.ToString().Substring(6)) + 1;
            CargarCombos();
            return View();
        }

        [HttpPost]
        public ActionResult Nuevo(ConvocatoriaDTO entidad)
        {
            try
            {
                var mensaje = string.Empty;
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
                    Requisito = null
                };
                if (entidad.Convocatoriaid <= 0)
                {
                    db.DB_Convocatoria.Add(convocatoria);
                    db.SaveChanges();
                    mensaje = "Se registro con exito";
                }
                else
                {
                    db.Entry(convocatoria).State = EntityState.Modified;
                    db.SaveChanges();
                    mensaje = "Se actualizo con exito";
                }
                return Json(new { statusCode = HttpStatusCode.OK, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
            } catch (Exception ex)
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
                entidad.Estado = "S";
                entidad.FechaSuspension = DateTime.Now;
                db.Entry(entidad).State = EntityState.Modified;
                db.SaveChanges();
                correo.EnviarCorreo("Clinica Ricardo Palma", entidad.Empleado.Correo, "Suspension", "Correo de prueba", false, null);
                mensaje = "Se actualizo con exito";
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