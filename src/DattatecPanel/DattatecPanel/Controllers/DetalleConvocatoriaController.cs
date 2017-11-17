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
    public class DetalleConvocatoriaController : Controller
    {
        private ClinicaDBContext db = new ClinicaDBContext();
        private MailSMTP correo = new MailSMTP();
        //
        // GET: /DetalleConvocatoria/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarDetalleConvocatoriaPostulante(string ruc, string razonSocial)
        {
            try
            {
                var lista = new DetalleConvocatoriaModel().ListarDetalleConvocatoriaPostulante(ruc,razonSocial);
                var jsonresult = Json(new { rows = lista }, JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = int.MaxValue;
                return jsonresult;
            }
            catch (Exception ex)
            {
                return Json(new { statusCode = HttpStatusCode.BadRequest }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult Validar(int id)
        {
            var entidad = db.DB_DetalleConvocatoria.Select(x => new {x.Postulante.PostulanteId, x.Postulante.RUC, x.Postulante.RazonSocial, x.Convocatoria.Rubro.Descripcion, x.Postulante.Correo });
                entidad = entidad.Where(s => s.PostulanteId == id);
            
            return View("Validar", entidad);
        }

        [HttpPost]
        public ActionResult Suspender(Convocatoria entidad)
        {
            try
            {
                var mensaje = string.Empty;
                //if (string.IsNullOrEmpty(entidad.ObservacionSuspension))
                //{
                //    return Json(new { statusCode = HttpStatusCode.OK, mensaje = "Ingrese una observación." }, JsonRequestBehavior.AllowGet);
                //}
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



	}
}