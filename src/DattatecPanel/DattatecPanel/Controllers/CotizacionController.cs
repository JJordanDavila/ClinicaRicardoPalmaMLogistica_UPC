using DattatecPanel.Context;
using DattatecPanel.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DattatecPanel.Controllers
{
    public class CotizacionController : Controller
    {
        private ClinicaDBContext db = new ClinicaDBContext();

        public ActionResult BuscarAprobadas(string numeroocc, string numerorqc, int? proveedoresc, int? areasc, DateTime? fechainicioc, DateTime? fechafinc)
        {
            ViewBag.ProveedoresC = new SelectList(db.DB_Proveedor, "ProveedorID", "RazonSocial");
            ViewBag.AreasC = new SelectList(db.DB_Areas, "AreaID", "Descripcion");
            var cotizacionAprobadas = db.DB_Cotizacion.Where(c => c.TransaccionCompra.Estado == "AC");

            if (numeroocc != null && !String.IsNullOrEmpty(numeroocc))
            {
                cotizacionAprobadas = cotizacionAprobadas.Where(c =>
                                                c.TransaccionCompra.Numero.Contains(numeroocc.Trim()));
            }

            if (numerorqc != null && !String.IsNullOrEmpty(numerorqc))
            {
                cotizacionAprobadas = cotizacionAprobadas.Where(c =>
                                                c.RequerimientoCompra.TransaccionCompra.Numero.Contains(numerorqc.Trim()));
            }

            if (proveedoresc != null)
            {
                cotizacionAprobadas = cotizacionAprobadas.Where(c =>
                                                c.Proveedor.ProveedorID == proveedoresc);
            }

            if (areasc != null)
            {
                cotizacionAprobadas = cotizacionAprobadas.Where(c =>
                                                c.RequerimientoCompra.Solicitante.Area.AreaID == areasc);
            }

            if (fechainicioc != null && fechafinc != null)
            {
                cotizacionAprobadas = cotizacionAprobadas.Where(c =>
                                                c.TransaccionCompra.Fecha >= fechainicioc &&
                                                c.TransaccionCompra.Fecha <= fechafinc);
            }
            return View(cotizacionAprobadas.ToList());
        }

        // GET: Cotizacion
        public ActionResult Index()
        {
            var dB_Cotizacion = db.DB_Cotizacion.Include(c => c.FormaPago).Include(c => c.Proveedor).Include(c => c.RequerimientoCompra);
            return View(dB_Cotizacion.ToList());
        }

        // GET: Cotizacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cotizacion cotizacion = db.DB_Cotizacion.Find(id);
            if (cotizacion == null)
            {
                return HttpNotFound();
            }
            return View(cotizacion);
        }

        // GET: Cotizacion/Create
        public ActionResult Create()
        {
            ViewBag.FormaPagoID = new SelectList(db.DB_FormaPago, "FormaPagoID", "Nombre");
            ViewBag.ProveedorID = new SelectList(db.DB_Proveedor, "ProveedorID", "NombreComercial");
            ViewBag.RequerimientoCompraID = new SelectList(db.DB_RequerimientoCompra, "RequerimientoCompraID", "RequerimientoCompraID");
            return View();
        }

        // POST: Cotizacion/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CotizacionID,RequerimientoCompraID,ProveedorID,FormaPagoID,PlazoEntrega,TiempoValidez,NroReferencia")] Cotizacion cotizacion)
        {
            if (ModelState.IsValid)
            {
                db.DB_Cotizacion.Add(cotizacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FormaPagoID = new SelectList(db.DB_FormaPago, "FormaPagoID", "Nombre", cotizacion.FormaPagoID);
            ViewBag.ProveedorID = new SelectList(db.DB_Proveedor, "ProveedorID", "NombreComercial", cotizacion.ProveedorID);
            ViewBag.RequerimientoCompraID = new SelectList(db.DB_RequerimientoCompra, "RequerimientoCompraID", "RequerimientoCompraID", cotizacion.RequerimientoCompraID);
            return View(cotizacion);
        }

        // GET: Cotizacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cotizacion cotizacion = db.DB_Cotizacion.Find(id);
            if (cotizacion == null)
            {
                return HttpNotFound();
            }
            ViewBag.FormaPagoID = new SelectList(db.DB_FormaPago, "FormaPagoID", "Nombre", cotizacion.FormaPagoID);
            ViewBag.ProveedorID = new SelectList(db.DB_Proveedor, "ProveedorID", "NombreComercial", cotizacion.ProveedorID);
            ViewBag.RequerimientoCompraID = new SelectList(db.DB_RequerimientoCompra, "RequerimientoCompraID", "RequerimientoCompraID", cotizacion.RequerimientoCompraID);
            return View(cotizacion);
        }

        // POST: Cotizacion/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CotizacionID,RequerimientoCompraID,ProveedorID,FormaPagoID,PlazoEntrega,TiempoValidez,NroReferencia")] Cotizacion cotizacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cotizacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FormaPagoID = new SelectList(db.DB_FormaPago, "FormaPagoID", "Nombre", cotizacion.FormaPagoID);
            ViewBag.ProveedorID = new SelectList(db.DB_Proveedor, "ProveedorID", "NombreComercial", cotizacion.ProveedorID);
            ViewBag.RequerimientoCompraID = new SelectList(db.DB_RequerimientoCompra, "RequerimientoCompraID", "RequerimientoCompraID", cotizacion.RequerimientoCompraID);
            return View(cotizacion);
        }

        // GET: Cotizacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cotizacion cotizacion = db.DB_Cotizacion.Find(id);
            if (cotizacion == null)
            {
                return HttpNotFound();
            }
            return View(cotizacion);
        }

        // POST: Cotizacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cotizacion cotizacion = db.DB_Cotizacion.Find(id);
            db.DB_Cotizacion.Remove(cotizacion);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}