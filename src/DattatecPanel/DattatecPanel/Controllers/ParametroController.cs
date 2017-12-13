using DattatecPanel.Context;
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
    public class ParametroController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ClinicaDBContext db = new ClinicaDBContext();
        //
        // GET: /Parametro/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarParametros(int page, int pageSize)
        {
            try
            {
                var lista = new ParametroModel().ListarParametros(page, pageSize);
                var jsonresult = Json(new { rows = lista.lista, total = lista.total }, JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = int.MaxValue;
                return jsonresult;
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Nuevo()
        {
            ////var numerogenerado = string.Empty;
            ////if (db.DB_Parametro.Count() > 0)
            ////{
            ////    var Parametro = db.DB_Parametro.OrderByDescending(x => x.Numero).First();
            ////    if (Parametro != null)
            ////    {
            ////        var numero = Parametro.Numero;
            ////        var correlativo = Convert.ToInt32(numero.ToString().Substring(6)) + 1;
            ////        numerogenerado = numero.Substring(0, 6) + correlativo.ToString().PadLeft(6, '0');
            ////    }
            ////}
            ////else
            ////{
            ////    numerogenerado = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + "000001";
            ////}
            ////ViewBag.NuevoNumeroParametro = numerogenerado;
            ////CargarCombos();
            ViewBag.Accion = "Nuevo";
            return View();
        }

        [HttpPost]
        public ActionResult Nuevo(ParametroDTO entidad)
        {
            try
            {
                var mensaje = string.Empty;

                //////Parametro Parametro = new Parametro
                //////{
                //////    ParametroId = entidad.ParametroId,
                //////    FecIni = entidad.FecIni,
                //////    FecFin = entidad.FecFin,
                //////    Intervalo = entidad.Intervalo,
                //////    UnidadMedidaIntervalo = entidad.UnidadMedidaIntervalo,
                //////    FecUltPro = entidad.FecUltPro,
                //////    UrlServicio01 = entidad.UrlServicio01,
                //////    UrlServicio02 = entidad.UrlServicio02
                //////};
                //////if (entidad.ParametroId <= 0)
                //////{
                //////    db.DB_Parametro.Add(Parametro);
                //////    db.SaveChanges();
                //////    mensaje = "Se registro con exito";
                //////}
                //////else
                //////{

                //////}

                var response = new ParametroModel().GuardarParametro(entidad);
                return Json(new { statusCode = HttpStatusCode.OK, mensaje = response.mensaje, mensajeInfo = response.mensajeInfo },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        ////public ActionResult Actualizar(int id)
        ////{
        ////    var entidad = db.DB_Parametro.Where(x => x.ParametroId == id).First();
        ////    //CargarCombos();
        ////    return View("Nuevo", entidad);
        ////}

        public ActionResult Suspender(int id)
        {
            var entidad = db.DB_Parametro.Where(x => x.ParametroId == id).First();
            //CargarCombos();
            return View("Suspender", entidad);
        }

        [HttpPost]
        public ActionResult Suspender(Parametro entidad)
        {
            try
            {
                var mensaje = string.Empty;
                //if (string.IsNullOrEmpty(entidad.ObservacionSuspension))
                //{
                //    return Json(new { statusCode = HttpStatusCode.OK, mensaje = "Ingrese una observación." }, JsonRequestBehavior.AllowGet);
                //}
                ////var cuerpoCorreo = "Se suspendio la convocatoria con el numero : " + entidad.Numero.ToString();
                //////var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == entidad.EmpleadoID).FirstOrDefault();

                var parametro = db.DB_Parametro.Where(x => x.ParametroId == entidad.ParametroId).FirstOrDefault();
                parametro.FecIni = entidad.FecIni;
                parametro.FecFin = entidad.FecFin;
                parametro.Intervalo = entidad.Intervalo;
                parametro.UnidadMedidaIntervalo = entidad.UnidadMedidaIntervalo;
                parametro.FecUltPro = entidad.FecUltPro;
                parametro.UrlServicio01 = entidad.UrlServicio01;
                parametro.UrlServicio02 = entidad.UrlServicio02;

                db.Entry(parametro).State = EntityState.Modified;
                db.SaveChanges();
                ////correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Suspension de convocatoria", cuerpoCorreo, false, null);
                mensaje = "Los datos se modificaron correctamente";
                return Json(new { statusCode = HttpStatusCode.OK, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Eliminar(int id)
        {
            //Parametro parametro = db.DB_Parametro.Find(id);
            ////db.DB_Parametro.Remove(parametro);
            ////db.SaveChanges();
            ////return RedirectToAction("Index");

            ViewBag.Accion = "Eliminar";
            var entidad = db.DB_Parametro.Where(x => x.ParametroId == id).First();
            return View("Nuevo", entidad);
        }

        [HttpPost]
        public ActionResult Eliminar(Parametro entidad)
        {
            try
            {
                var mensaje = string.Empty;
                Parametro parametro = db.DB_Parametro.Find(entidad.ParametroId);
                db.DB_Parametro.Remove(parametro);
                db.SaveChanges();

                mensaje = "Los datos se eliminaron correctamente";
                return Json(new { statusCode = HttpStatusCode.OK, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        //////////////////////////////////
        public ActionResult Actualizar(int id)
        {
            var entidad = db.DB_Parametro.Where(x => x.ParametroId == id).First();
            //CargarCombos();
            ViewBag.Accion = "Actualizar";
            return View("Nuevo", entidad);
        }

        [HttpPost]
        public ActionResult Actualizar(Parametro entidad)
        {
            try
            {
                var mensaje = string.Empty;
                var parametro = db.DB_Parametro.Where(x => x.ParametroId == entidad.ParametroId).FirstOrDefault();
                parametro.FecIni = entidad.FecIni;
                parametro.FecFin = entidad.FecFin;
                parametro.Intervalo = entidad.Intervalo;
                parametro.UnidadMedidaIntervalo = entidad.cbxMedidasIntervalos;
                parametro.FecUltPro = entidad.FecUltPro;
                parametro.UrlServicio01 = entidad.UrlServicio01;
                parametro.UrlServicio02 = entidad.UrlServicio02;

                db.Entry(parametro).State = EntityState.Modified;
                db.SaveChanges();
                ////correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Suspension de convocatoria", cuerpoCorreo, false, null);
                mensaje = "Los datos se modificaron correctamente";
                return Json(new { statusCode = HttpStatusCode.OK, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult Historial()
        {
            return View("Historial");
        }

        public ActionResult ListarHistorial(string fini, string ffin, int page, int pageSize)
        {
            try
            {
                var lista = new ParametroModel().ListarHistorial(fini, ffin, page, pageSize);
                return Json(new { statusCode = HttpStatusCode.OK, rows = lista.lista, total = lista.total }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest, mensaje = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}