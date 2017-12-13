using DattatecPanel.Context;
using DattatecPanel.Models;
using DattatecPanel.Models.DTO;
using System;
using System.Net;
using System.Web.Mvc;

namespace DattatecPanel.Controllers
{
    public class ProveedorController : Controller
    {
        private ClinicaDBContext db = new ClinicaDBContext();

        // GET: Proveedor
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HistorialProveedor(int? proveedorId)
        {
            HistorialProveedorDTO historial = new ProveedorModel().HistorialProveedor(proveedorId);
            return View("DetalleHistorial", historial);
        }

        public ActionResult ListarProveedor(string ruc, string razonSocial, int page, int pageSize)
        {
            try
            {
                var lista = new ProveedorModel().ListarProveedor(ruc, razonSocial, page, pageSize);
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