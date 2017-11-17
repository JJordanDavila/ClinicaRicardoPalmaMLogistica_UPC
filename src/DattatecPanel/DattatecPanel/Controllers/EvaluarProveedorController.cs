using DattatecPanel.Context;
using DattatecPanel.Models;
using DattatecPanel.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DattatecPanel.Controllers
{
    public class EvaluarProveedorController : Controller
    {
        private ClinicaDBContext db = new ClinicaDBContext();
        // GET: EvaluarProveedor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListarProveedores(RequestEvaluarProveedor request)
        {
            try
            {
                var lista = new EvaluarProveedorModel().ListarProveedores(request);
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