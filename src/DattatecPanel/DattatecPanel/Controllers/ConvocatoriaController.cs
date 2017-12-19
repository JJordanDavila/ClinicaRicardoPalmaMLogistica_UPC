using DattatecPanel.Models;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Entidades;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DattatecPanel.Controllers
{
    public class ConvocatoriaController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        // GET: Convocatoria
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarConvocatoriaProveedores(int page, int pageSize, string numero, string fini, string ffin)
        {
            try
            {
                var validacion = new ConvocatoriaModel().ValidarFiltros(numero, fini, ffin);
                if (!string.IsNullOrEmpty(validacion))
                {
                    return Json(new { mensaje = validacion, statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
                }
                var lista = new ConvocatoriaModel().ListarConvocatoriaProveedores(numero, fini, ffin, page, pageSize);
                //var jsonresult = Json(new { rows = lista.lista, total = lista.total }, JsonRequestBehavior.AllowGet);
                //jsonresult.MaxJsonLength = int.MaxValue;
                //return jsonresult;
                return Json(new { rows = lista.lista, total = lista.total, statusCode = HttpStatusCode.OK }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { mensaje = ex.Message, statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
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
                var validacion = new ConvocatoriaModel().ValidarFiltros(entidad.Numero, entidad.FechaInicio.ToString(), entidad.FechaFin.ToString());
                if (!string.IsNullOrEmpty(validacion))
                {
                    return Json(new { mensajeInfo = validacion, statusCode = HttpStatusCode.OK }, JsonRequestBehavior.AllowGet);
                }
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
            var entidad = new ConvocatoriaModel().ObtenerConvocatoriaPorID(id);
            CargarCombos();
            return View("Nuevo", entidad);
        }

        public ActionResult Suspender(int id)
        {
            var entidad = new ConvocatoriaModel().ObtenerConvocatoriaPorID(id);
            CargarCombos();
            return View("Suspender", entidad);
        }

        [HttpPost]
        public ActionResult Suspender(Convocatoria entidad)
        {
            try
            {
                var response = new ConvocatoriaModel().GuardarSuspension(entidad);
                return Json(new { statusCode = HttpStatusCode.OK,
                    mensaje = response.mensaje, mensajeInfo = response.mensajeInfo
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        private void CargarCombos()
        {
            ViewBag.Rubros = new ConvocatoriaModel().ListarRubros();
            ViewBag.Solicitantes = new ConvocatoriaModel().ListarEmpleados();
        }
    }
}