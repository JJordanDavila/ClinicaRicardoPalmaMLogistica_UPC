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
using System.Web.Mvc;

namespace DattatecPanel.Controllers
{
    public class ConvocatoriaController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ClinicaDBContext db = new ClinicaDBContext();
        private MailSMTP correo = new MailSMTP();
        // GET: Convocatoria
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarConvocatoriaProveedores(int page, int pageSize, string numero, string fini, string ffin)
        {
            try
            {
                var lista = new ConvocatoriaModel().ListarConvocatoriaProveedores(numero, fini, ffin);
                var jsonresult = Json(new { rows = lista }, JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = int.MaxValue;
                return jsonresult;
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Nuevo()
        {
            ViewBag.NuevoNumeroConvocatoria = new ConvocatoriaModel().GenerarNumeroCorrelativo();
            CargarCombos();
            return View();
        }

        [HttpPost]
        public ActionResult Nuevo(ConvocatoriaDTO entidad)
        {
            try
            {
                var response = new ConvocatoriaModel().GuardarConvocatoria(entidad);
                return Json(new { statusCode = HttpStatusCode.OK, mensaje = response.mensaje, mensajeInfo = response.mensajeInfo }, 
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
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
                    return Json(new { statusCode = HttpStatusCode.OK, mensaje = "", mensajeInfo = "Ingrese una observación" }, JsonRequestBehavior.AllowGet);
                }
                var cuerpoCorreo = "Se suspendio la convocatoria con el numero : " + entidad.Numero.ToString();
                var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == entidad.EmpleadoID).FirstOrDefault();
                entidad.Estado = "S";
                entidad.FechaSuspension = DateTime.Now;
                db.Entry(entidad).State = EntityState.Modified;
                db.SaveChanges();
                correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Suspension de convocatoria", cuerpoCorreo, false, null);
                mensaje = "Se suspendio con exito";
                return Json(new { statusCode = HttpStatusCode.OK, mensaje = mensaje, mensajeInfo = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        private void CargarCombos()
        {
            ViewBag.Rubros = db.DB_Rubro.ToList();
            ViewBag.Solicitantes = db.DB_Empleado.ToList();
        }
    }
}