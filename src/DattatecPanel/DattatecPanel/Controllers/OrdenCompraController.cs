using DattatecPanel.Context;
using DattatecPanel.Models;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Util;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace DattatecPanel.Controllers
{
    public class OrdenCompraController : Controller
    {
        private ClinicaDBContext db = new ClinicaDBContext();

        // GET: OrdenCompras
        public ActionResult Index(string numerooc, string numerorq, int? proveedores, int? areas, DateTime? fechainicio, DateTime? fechafin)
        {
            ViewBag.Proveedores = new SelectList(db.DB_Proveedor, "ProveedorID", "RazonSocial");
            ViewBag.Areas = new SelectList(db.DB_Areas, "AreaID", "Descripcion");
            List<OrdenCompra> lstOrdenCompra = db.DB_OrdenCompra.Where(
                                                c => c.TransaccionCompra.Estado != "AN").OrderByDescending(d => d.TransaccionCompra.Fecha).ToList();

            if (numerooc != null && !String.IsNullOrEmpty(numerooc))
            {
                lstOrdenCompra = lstOrdenCompra.Where(c =>
                                                c.TransaccionCompra.Numero.Contains(numerooc.Trim())).OrderByDescending(d => d.TransaccionCompra.Fecha).ToList();
            }

            if (numerorq != null && !String.IsNullOrEmpty(numerorq))
            {
                lstOrdenCompra = lstOrdenCompra.Where(c =>
                                                c.Cotizacion.RequerimientoCompra.TransaccionCompra.Numero.Contains(numerorq.Trim())).OrderByDescending(d => d.TransaccionCompra.Fecha).ToList();
            }

            if (proveedores != null)
            {
                lstOrdenCompra = lstOrdenCompra.Where(c =>
                                                c.Cotizacion.Proveedor.ProveedorID == proveedores).OrderByDescending(d => d.TransaccionCompra.Fecha).ToList();
            }

            if (areas != null)
            {
                lstOrdenCompra = lstOrdenCompra.Where(c =>
                                                c.Cotizacion.RequerimientoCompra.Solicitante.Area.AreaID == areas).OrderByDescending(d => d.TransaccionCompra.Fecha).ToList();
            }

            if (fechainicio != null && fechafin != null)
            {
                lstOrdenCompra = lstOrdenCompra.Where(c =>
                                                c.TransaccionCompra.Fecha >= fechainicio &&
                                                c.TransaccionCompra.Fecha <= fechafin).OrderByDescending(d => d.TransaccionCompra.Fecha).ToList();
            }
            return View(lstOrdenCompra);
        }

        // GET: OrdenCompras/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrdenCompra ordenCompra = db.DB_OrdenCompra.Find(id);
            if (ordenCompra == null)
            {
                return HttpNotFound();
            }
            ordenCompra.lstDetalle = db.DB_DetalleTransaccionCompra.Where(c => c.TransaccionCompraID == ordenCompra.OrdenCompraID).ToList();
            return View(ordenCompra);
        }

        public ActionResult ObtenerReporte(string id, int idOC)
         {
             LocalReport lr = new LocalReport();
             string path = Path.Combine(Server.MapPath("~/ReportsRDLC"), "rptDetalleOrdenCompra.rdlc");
             if (System.IO.File.Exists(path))
             {
                 lr.ReportPath = path;
             }
             else
             {
                 return View("Details");
             }

             OrdenCompra ordenCompra = db.DB_OrdenCompra.Find(idOC);
             List<OrdenCompraDTO> dtOC = new List<OrdenCompraDTO>();

             OrdenCompraDTO dtOCc = new OrdenCompraDTO
             {
                 CodigoDetalle = ordenCompra.OrdenCompraID.ToString(),
                 NumeroOC = ordenCompra.TransaccionCompra.Numero,
                 RazonSocial = ordenCompra.Cotizacion.Proveedor.RazonSocial,
                 Fecha = ordenCompra.TransaccionCompra.Fecha,
                 Area = ordenCompra.Cotizacion.RequerimientoCompra.Solicitante.Area.Descripcion,
                 Solicitante = ordenCompra.Cotizacion.RequerimientoCompra.Solicitante.NombreCompleto,
                 Direccion = ordenCompra.Almacen.Direccion,
                 Observacion = ordenCompra.Observacion,
                 FechaEntrega = (DateTime)ordenCompra.FechaEntrega,
                 AñoActual = DateTime.Now.Year
             };
             dtOC.Add(dtOCc);

             ordenCompra.lstDetalle = db.DB_DetalleTransaccionCompra.Where(c => c.TransaccionCompraID == ordenCompra.OrdenCompraID).ToList();
             List<DetalleOrdenCompraDTO> dtDOC = new List<DetalleOrdenCompraDTO>();

             foreach (var item in ordenCompra.lstDetalle.ToList())
             {
                 var doc = new DetalleOrdenCompraDTO
                 {
                     Item = item.DetalleTransaccionCompraID.ToString(),
                     Producto = item.Articulo.Descripcion,
                     Cantidad = item.Cantidad,
                     PrecioVenta = item.Precio,
                     Total = item.Total
                 };
                 dtDOC.Add(doc);
             }

             ReportDataSource rdOC = new ReportDataSource("dsOrdenCompra", dtOC);
             ReportDataSource rdDOC = new ReportDataSource("dsDetalleOC", dtDOC);
             lr.DataSources.Add(rdOC);
             lr.DataSources.Add(rdDOC);
             string reportType = id;
             string mimeType;
             string encoding;
             string fileNameExtension;

             string deviceInfo =

             "<DeviceInfo>" +
             "  <OutputFormat>" + id + "</OutputFormat>" +
             "  <PageWidth>8.5in</PageWidth>" +
             "  <PageHeight>11in</PageHeight>" +
             "  <MarginTop>0.5in</MarginTop>" +
             "  <MarginLeft>1in</MarginLeft>" +
             "  <MarginRight>1in</MarginRight>" +
             "  <MarginBottom>0.5in</MarginBottom>" +
             "</DeviceInfo>";

             Warning[] warnings;
             string[] streams;
             byte[] renderedBytes;

             renderedBytes = lr.Render(
                 reportType,
                 deviceInfo,
                 out mimeType,
                 out encoding,
                 out fileNameExtension,
                 out streams,
                 out warnings);
             return File(renderedBytes, mimeType);
         }

        // GET: OrdenCompras/Create
        public ActionResult Create(int? idCO)
        {
            int id = (int)idCO;
            OrdenCompra ordenCompra = new OrdenCompra();
            ordenCompra.CotizacionID = id;
            ordenCompra.FechaEntrega = null;
            ordenCompra.lstDetalle = db.DB_DetalleTransaccionCompra.Where(c => c.TransaccionCompraID == id).ToList();
            ViewBag.AlmacenID = new SelectList(db.DB_Almacen, "AlmacenID", "Direccion");
            ViewBag.MessageAdvCreateOC = false;
            return View(ordenCompra);
        }

        public string BodyProveedor(byte tipo, Cotizacion cotizacion, List<DetalleTransaccionCompra> lstDetalle, TransaccionCompra transaccion, OrdenCompra ordenCompra)
        {
            StringBuilder bodyProveedor = new StringBuilder();
            bodyProveedor.Append(string.Format("Estimado {0}:", cotizacion.Proveedor.RazonSocial));
            bodyProveedor.Append("<br></br>");
            bodyProveedor.Append("<br></br>");
            if (tipo == 0)
            {
                bodyProveedor.Append("Se envía el detalle de la Orden de Compra generada:");
            }
            else if (tipo == 1)
            {
                bodyProveedor.Append("Se envía el detalle de la Orden de Compra modificada:");
            }
            else
            {
                bodyProveedor.Append("Se envía el detalle de la Orden de Compra anulada:");
            }
            bodyProveedor.Append("<br></br>");
            bodyProveedor.Append("<br></br>");
            bodyProveedor.Append("<b>Número: </b>" + transaccion.Numero);
            bodyProveedor.Append("<br></br>");
            bodyProveedor.Append("<br></br>");
            bodyProveedor.Append("<b>Fecha: </b>" + transaccion.Fecha);
            bodyProveedor.Append("<br></br>");
            bodyProveedor.Append("<br></br>");
            bodyProveedor.Append("<b>Almacén: </b>" + ordenCompra.Almacen.Direccion);
            bodyProveedor.Append("<br></br>");
            bodyProveedor.Append("<br></br>");
            bodyProveedor.Append("<b>Fecha de Entrega: </b>" + ordenCompra.FechaEntrega);
            bodyProveedor.Append("<br></br>");
            bodyProveedor.Append("<br></br>");
            bodyProveedor.Append("<b>Observaciones: </b>" + ordenCompra.Observacion);
            bodyProveedor.Append("<br></br>");
            bodyProveedor.Append("<br></br>");
            if (tipo == 2)
            {
                bodyProveedor.Append("<b>Motivo: </b>" + ordenCompra.ObservacionAnulacion);
                bodyProveedor.Append("<br></br>");
                bodyProveedor.Append("<br></br>");
            }
            bodyProveedor.Append(@"<table style=""width:100%;font-family:Calibri,Verdana,Arial;"">");
            bodyProveedor.Append("<tr>");
            bodyProveedor.Append(@"<th style=""text-align: left;"">Item</th>");
            bodyProveedor.Append(@"<th style=""text-align: left;"">Producto</th>");
            bodyProveedor.Append(@"<th style=""text-align: right;"">Cantidad</th>");
            bodyProveedor.Append(@"<th style=""text-align: right;"">P. Venta</th>");
            bodyProveedor.Append(@"<th style=""text-align: right;"">Total</th>");
            bodyProveedor.Append("</tr>");
            foreach (DetalleTransaccionCompra item in lstDetalle)
            {
                bodyProveedor.Append("<tr>");
                bodyProveedor.Append(@"<td style=""text-align: left;"">" + item.DetalleTransaccionCompraID + "</td>");
                bodyProveedor.Append(@"<td style=""text-align: left;"">" + item.Articulo.Descripcion + "</td>");
                bodyProveedor.Append(@"<td style=""text-align: right;"">" + item.Cantidad + "</td>");
                bodyProveedor.Append(@"<td style=""text-align: right;"">" + item.Precio + "</td>");
                bodyProveedor.Append(@"<td style=""text-align: right;"">" + item.Total + "</td>");
                bodyProveedor.Append("</tr>");
            }
            bodyProveedor.Append("</table>");
            return bodyProveedor.ToString();
        }

        public string BodySolicitante(byte tipo, Cotizacion cotizacion, TransaccionCompra transaccion)
        {
            StringBuilder bodySolicitante = new StringBuilder();
            bodySolicitante.Append(string.Format("Estimado {0}:", cotizacion.RequerimientoCompra.Solicitante.NombreCompleto));
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");
            if (tipo == 0)
            {
                bodySolicitante.Append(string.Format("Se ha generado la Orden de Compra N° {0} según su Requerimiento de Compra N° {1}", transaccion.Numero, cotizacion.RequerimientoCompra.TransaccionCompra.Numero));
            }
            else if (tipo == 1)
            {
                bodySolicitante.Append(string.Format("Se ha modificado la Orden de Compra N° {0} relacionada a su Requerimiento de Compra N° {1}", transaccion.Numero, cotizacion.RequerimientoCompra.TransaccionCompra.Numero));
            }
            else
            {
                bodySolicitante.Append(string.Format("Se ha anulado la Orden de Compra N° {0} relacionada a su Requerimiento de Compra N° {1}", transaccion.Numero, cotizacion.RequerimientoCompra.TransaccionCompra.Numero));
            }
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");
            return bodySolicitante.ToString();
        }

        // POST: OrdenCompras/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CotizacionID,AlmacenID,Observacion,FechaEntrega")] OrdenCompra ordenCompra)
        {
            if (ModelState.IsValid)
            {
                if (ordenCompra.FechaEntrega < DateTime.Now)
                {
                    ViewBag.MessageAdvCreateOC = true;
                    ViewBag.MessageAdvertenciaOC = "La fecha de entrega no debe ser menor a la fecha actual.";
                    ViewBag.AlmacenID = new SelectList(db.DB_Almacen, "AlmacenID", "Direccion");
                    ordenCompra.lstDetalle = db.DB_DetalleTransaccionCompra.Where(c => c.TransaccionCompraID == ordenCompra.CotizacionID).ToList();

                    return View(ordenCompra);
                }

                if (ordenCompra.FechaEntrega > DateTime.Now.AddDays(120))
                {
                    ViewBag.MessageAdvCreateOC = true;
                    ViewBag.MessageAdvertenciaOC = "La fecha de entrega no debe ser mayor a 120 dias de la fecha actual.";
                    ViewBag.AlmacenID = new SelectList(db.DB_Almacen, "AlmacenID", "Direccion");
                    ordenCompra.lstDetalle = db.DB_DetalleTransaccionCompra.Where(c => c.TransaccionCompraID == ordenCompra.CotizacionID).ToList();

                    return View(ordenCompra);
                }
                int i = 0;
                string tipo = "OC";
                Cotizacion cotizacion = db.DB_Cotizacion.Find(ordenCompra.CotizacionID);
                int id = db.Database.SqlQuery<int>("dbo.Spl_GetIdTra").First();
                string numero = db.Database.SqlQuery<string>("dbo.Spl_GetNumero @p0", tipo).First();
                List<DetalleTransaccionCompra> lstDetalle = db.DB_DetalleTransaccionCompra.Where(c => c.TransaccionCompraID == cotizacion.CotizacionID).ToList();
                TransaccionCompra transaccionCO = cotizacion.TransaccionCompra;
                TransaccionCompra transaccionRQ = cotizacion.RequerimientoCompra.TransaccionCompra;
                //TRANSACCION
                TransaccionCompra transaccion = new TransaccionCompra()
                {
                    TransaccionCompraID = id,
                    Estado = "P",
                    Fecha = DateTime.Now,
                    MonedaID = transaccionCO.MonedaID,
                    Numero = numero
                };
                //DETALLE
                List<DetalleTransaccionCompra> lstTransaccion = new List<DetalleTransaccionCompra>();
                foreach (DetalleTransaccionCompra item in lstDetalle)
                {
                    i += 1;
                    lstTransaccion.Add(new DetalleTransaccionCompra()
                    {
                        TransaccionCompraID = id,
                        DetalleTransaccionCompraID = i,
                        ArticuloID = item.ArticuloID,
                        Cantidad = item.Cantidad,
                        Precio = item.Precio
                    });
                }
                //ORDEN DE COMPRA
                ordenCompra.OrdenCompraID = id;
                ordenCompra.Almacen = db.DB_Almacen.Find(ordenCompra.AlmacenID);
                //CAMBIA ESTADO COTIZACION y REQUERIMIENTO
                transaccionCO.Estado = "AT";
                transaccionRQ.Estado = "AT";
                //CAMBIOS EN LA BD
                db.DB_TransaccionCompra.Add(transaccion);
                foreach (DetalleTransaccionCompra item in lstTransaccion)
                {
                    db.DB_DetalleTransaccionCompra.Add(item);
                }
                db.DB_OrdenCompra.Add(ordenCompra);
                db.Entry(transaccionCO).State = EntityState.Modified;
                db.Entry(transaccionRQ).State = EntityState.Modified;
                db.SaveChanges();
                //ENVIO DE CORREO ELECTRONICO
                MailSMTP correo = new MailSMTP();
                string bodyProveedor = BodyProveedor(0, cotizacion, lstDetalle, transaccion, ordenCompra);
                string bodySolicitante = BodySolicitante(0, cotizacion, transaccion);
                correo.EnviarCorreo("Clínica Ricardo Palma", cotizacion.Proveedor.Correo, "Alerta CRP", bodyProveedor, true, null);
                correo.EnviarCorreo("Clínica Ricardo Palma", cotizacion.RequerimientoCompra.Solicitante.Correo, "Alerta CRP", bodySolicitante, true, null);
                return RedirectToAction("Index");
            }
            ViewBag.AlmacenID = new SelectList(db.DB_Almacen, "AlmacenID", "Direccion");
            ordenCompra.lstDetalle = db.DB_DetalleTransaccionCompra.Where(c => c.TransaccionCompraID == ordenCompra.CotizacionID).ToList();

            return View(ordenCompra);
        }

        // GET: OrdenCompras/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrdenCompra ordenCompra = db.DB_OrdenCompra.Find(id);
            if (ordenCompra == null)
            {
                return HttpNotFound();
            }
            ordenCompra.lstDetalle = db.DB_DetalleTransaccionCompra.Where(c => c.TransaccionCompraID == ordenCompra.OrdenCompraID).ToList();
            ViewBag.MessageAdvEditOC = false;
            return View(ordenCompra);
        }

        // POST: OrdenCompras/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrdenCompraID,Observacion,FechaEntrega")] OrdenCompra ordenCompra)
        {
            var ordenCompraaux = db.DB_OrdenCompra.Find(ordenCompra.OrdenCompraID);
            ordenCompraaux.FechaEntrega = ordenCompra.FechaEntrega;
            ordenCompraaux.Observacion = ordenCompra.Observacion;
            ordenCompra = ordenCompraaux;
            ordenCompra.lstDetalle = db.DB_DetalleTransaccionCompra.Where(c => c.TransaccionCompraID == ordenCompra.OrdenCompraID).ToList();

            if (ordenCompra.FechaEntrega < DateTime.Now)
            {
                ViewBag.MessageAdvEditOC = true;
                ViewBag.MessageAdvertenciaOC = "La fecha de entrega no debe ser menor a la fecha actual.";
                return View(ordenCompra);
            }

            if (ordenCompra.FechaEntrega > ordenCompraaux.TransaccionCompra.Fecha.AddDays(120))
            {
                ViewBag.MessageAdvEditOC = true;
                ViewBag.MessageAdvertenciaOC = "La fecha de entrega no debe ser mayor a 120 dias de la fecha de la transacción..";
                return View(ordenCompra);
            }

            if (ordenCompra.FechaEntrega < ordenCompraaux.TransaccionCompra.Fecha)
            {
                ViewBag.MessageAdvEditOC = true;
                ViewBag.MessageAdvertenciaOC = "La fecha de entrega no debe ser menor a la fecha de transacción.";
                return View(ordenCompra);
            }

            if (ModelState.IsValid)
            {
                OrdenCompra ordenCompraEdit = db.DB_OrdenCompra.Find(ordenCompra.OrdenCompraID);
                ordenCompraEdit.Observacion = ordenCompra.Observacion;
                ordenCompraEdit.FechaEntrega = ordenCompra.FechaEntrega;
                db.Entry(ordenCompraEdit).State = EntityState.Modified;
                db.SaveChanges();
                //ENVIO DE CORREO ELECTRONICO
                //MailSMTP correo = new MailSMTP();
                //string bodyProveedor = BodyProveedor(1, ordenCompraEdit.Cotizacion, ordenCompra.lstDetalle, ordenCompraEdit.TransaccionCompra, ordenCompraEdit);
                //string bodySolicitante = BodySolicitante(1, ordenCompraEdit.Cotizacion, ordenCompraEdit.TransaccionCompra);
                //correo.EnviarCorreo("Clínica Ricardo Palma", ordenCompraEdit.Cotizacion.Proveedor.Correo, "Alerta CRP", bodyProveedor, true, null);
                //correo.EnviarCorreo("Clínica Ricardo Palma", ordenCompraEdit.Cotizacion.RequerimientoCompra.Solicitante.Correo, "Alerta CRP", bodySolicitante, true, null);
                return RedirectToAction("Index");
            }
            return View(ordenCompra);
        }

        // GET: OrdenCompras/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrdenCompra ordenCompra = db.DB_OrdenCompra.Find(id);
            if (ordenCompra == null)
            {
                return HttpNotFound();
            }
            return View(ordenCompra);
        }

        // POST: OrdenCompras/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete([Bind(Include = "OrdenCompraID,ObservacionAnulacion")] OrdenCompra ordenCompra)
        {
            OrdenCompra ordenCompraEdit = db.DB_OrdenCompra.Find(ordenCompra.OrdenCompraID);
            if (string.IsNullOrWhiteSpace(ordenCompra.ObservacionAnulacion))
            {
                ModelState.AddModelError("ObservacionAnulacion", "El campo Motivo es obligatorio.");
            }
            else
            {
                ordenCompra.lstDetalle = db.DB_DetalleTransaccionCompra.Where(c => c.TransaccionCompraID == ordenCompraEdit.OrdenCompraID).ToList();
                TransaccionCompra transaccionOC = ordenCompraEdit.TransaccionCompra;
                TransaccionCompra transaccionCO = ordenCompraEdit.Cotizacion.TransaccionCompra;
                TransaccionCompra transaccionRQ = ordenCompraEdit.Cotizacion.RequerimientoCompra.TransaccionCompra;
                //CAMBIA ESTADO COTIZACION y REQUERIMIENTO
                transaccionCO.Estado = "AC";
                transaccionRQ.Estado = "PE";
                //ANULACION DE OC
                transaccionOC.Estado = "AN";
                transaccionOC.ObservacionAnulacion = ordenCompra.ObservacionAnulacion;
                ordenCompraEdit.ObservacionAnulacion = ordenCompra.ObservacionAnulacion;
                db.Entry(transaccionOC).State = EntityState.Modified;
                db.Entry(transaccionCO).State = EntityState.Modified;
                db.Entry(transaccionRQ).State = EntityState.Modified;
                db.SaveChanges();
                //ENVIO DE CORREO ELECTRONICO
                MailSMTP correo = new MailSMTP();
                string bodyProveedor = BodyProveedor(2, ordenCompraEdit.Cotizacion, ordenCompra.lstDetalle, ordenCompraEdit.TransaccionCompra, ordenCompraEdit);
                string bodySolicitante = BodySolicitante(2, ordenCompraEdit.Cotizacion, ordenCompraEdit.TransaccionCompra);
                correo.EnviarCorreo("Clínica Ricardo Palma", ordenCompraEdit.Cotizacion.Proveedor.Correo, "Alerta CRP", bodyProveedor, true, null);
                correo.EnviarCorreo("Clínica Ricardo Palma", ordenCompraEdit.Cotizacion.RequerimientoCompra.Solicitante.Correo, "Alerta CRP", bodySolicitante, true, null);
                return RedirectToAction("Index");
            }
            return View(ordenCompraEdit);
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
