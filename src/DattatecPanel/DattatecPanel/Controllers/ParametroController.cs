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
    public class ParametroController : Controller
    {
        private ClinicaDBContext db = new ClinicaDBContext();
        //
        // GET: /Parametro/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarParametros()
        {
            try
            {
                var lista = new ParametroModel().ListarParametros();
                var jsonresult = Json(new { rows = lista }, JsonRequestBehavior.AllowGet);
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
            return View();
        }

        [HttpPost]
        public ActionResult Nuevo(ParametroDTO entidad)
        {
            try
            {
               
                var mensaje = string.Empty;
               
                Parametro Parametro = new Parametro
                {
                    ParametroId = entidad.ParametroId,
                    FecIni = entidad.FecIni,
                    FecFin = entidad.FecFin,
                    Intervalo = entidad.Intervalo,
                    UnidadMedidaIntervalo = entidad.UnidadMedidaIntervalo,
                    FecUltPro = entidad.FecUltPro,
                    UrlServicio01 = entidad.UrlServicio01,
                    UrlServicio02 = entidad.UrlServicio02
                };
                if (entidad.ParametroId <= 0)
                {
                    ////var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == Parametro.EmpleadoID).FirstOrDefault();
                    ////var cuerpoCorreo = "Se registro la Parametro con el numero : " + Parametro.Numero.ToString();
                    db.DB_Parametro.Add(Parametro);
                    db.SaveChanges();
                    ////correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Creación de Parametro", cuerpoCorreo, false, null);
                    mensaje = "Se registro con exito";
                }
                else
                {
                    //////var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == Parametro.EmpleadoID).FirstOrDefault();
                    //////var cuerpoCorreo = "Se actualizo la Parametro con el numero : " + Parametro.Numero.ToString();
                    //////db.Entry(Parametro).State = EntityState.Modified;
                    //////db.SaveChanges();
                    //////correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Actualizacion de Parametro", cuerpoCorreo, false, null);
                    //////mensaje = "Se actualizo con exito";
                }
                return Json(new { statusCode = HttpStatusCode.OK, mensaje = mensaje, mensajeInfo = "" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult Actualizar(int id)
        {
            var entidad = db.DB_Parametro.Where(x => x.ParametroId == id).First();
            //CargarCombos();
            return View("Nuevo", entidad);
        }

        public ActionResult Suspender(int id)
        {
            var entidad = db.DB_Parametro.Where(x => x.ParametroId == id).First();
            //CargarCombos();
            return View("Suspender", entidad);
        }

	}
}