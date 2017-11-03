using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DattatecPanel.Models;
using System.Net.Mail;
using System.IO;

namespace DattatecPanel.Controllers
{
    public class GS_SRSController : Controller
    {
        private DB_CLINICAEntities db = new DB_CLINICAEntities();

        // GET: GS_SRS
        public ActionResult Index()
        {
            var gS_SRS = db.GS_SRS.Include(g => g.GS_ASEGURADORA).Include(g => g.GS_ORDEN_MEDICA);
            return View(gS_SRS.ToList());
        }

        // GET: GS_SRS/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GS_SRS gS_SRS = db.GS_SRS.Find(id);
            if (gS_SRS == null)
            {
                return HttpNotFound();
            }
            return View(gS_SRS);
        }

        // GET: GS_SRS/Create
        public ActionResult Create()
        {
            ViewBag.IDAseguradora = new SelectList(db.GS_ASEGURADORA, "IDAseguradora", "Razon_social");
            ViewBag.IDOrdenMedica = new SelectList(db.GS_ORDEN_MEDICA, "IDOrdenMedica", "Estado");
            return View();
        }

        // POST: GS_SRS/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDSRS,IDAseguradora,IDOrdenMedica,Fecha_registro,Observacion,Estado,Fecha_aprobacion")] GS_SRS gS_SRS)
        {
            //if (ModelState.IsValid)
            //{
            //    db.GS_SRS.Add(gS_SRS);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            ViewBag.IDAseguradora = new SelectList(db.GS_ASEGURADORA, "IDAseguradora", "Razon_social", gS_SRS.IDAseguradora);
            ViewBag.IDOrdenMedica = new SelectList(db.GS_ORDEN_MEDICA, "IDOrdenMedica", "Estado", gS_SRS.IDOrdenMedica);
            return View(gS_SRS);
        }

        // GET: GS_SRS/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GS_SRS gS_SRS = db.GS_SRS.Find(id);
            if (gS_SRS == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDAseguradora = new SelectList(db.GS_ASEGURADORA, "IDAseguradora", "Ruc", gS_SRS.IDAseguradora);
            ViewBag.IDOrdenMedica = new SelectList(db.GS_ORDEN_MEDICA, "IDOrdenMedica", "Estado", gS_SRS.IDOrdenMedica);
            return View(gS_SRS);
        }

        // POST: GS_SRS/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDSRS,IDAseguradora,IDOrdenMedica,Fecha_registro,Observacion,Estado,Fecha_aprobacion")] GS_SRS gS_SRS)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gS_SRS).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDAseguradora = new SelectList(db.GS_ASEGURADORA, "IDAseguradora", "Ruc", gS_SRS.IDAseguradora);
            ViewBag.IDOrdenMedica = new SelectList(db.GS_ORDEN_MEDICA, "IDOrdenMedica", "Estado", gS_SRS.IDOrdenMedica);
            return View(gS_SRS);
        }

        // GET: GS_SRS/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GS_SRS gS_SRS = db.GS_SRS.Find(id);
            if (gS_SRS == null)
            {
                return HttpNotFound();
            }
            return View(gS_SRS);
        }

        // POST: GS_SRS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GS_SRS gS_SRS = db.GS_SRS.Find(id);
            db.GS_SRS.Remove(gS_SRS);
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

        public ActionResult ConsultaExterna(String nro_solicitud, String nro_dni)
        {
            var solicitud = from s in db.GS_SRS select s;
            if (!String.IsNullOrEmpty(nro_solicitud) && !String.IsNullOrEmpty(nro_dni))
            {
                int nrosolic = Convert.ToInt32(nro_solicitud);
                int dni = Convert.ToInt32(nro_dni);
                solicitud = solicitud.Where(p => p.IDSRS == nrosolic)
                                     .Where(x => x.GS_ORDEN_MEDICA.GS_ATENCION.GS_PACIENTE.IDPaciente == dni);

            }
            return View(solicitud);
        }

        public JsonResult SaveSRS(int? id,int? idx)
        {
            var nrosrs = db.SP_GENERAR_SRS(id, idx);           

            return Json(nrosrs.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerarPdfSolicitud(int id)
        {
            var pdfsol = db.SP_GENERAR_PDF_ASEGURADORA(id);
            return Json(pdfsol.ToArray(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DetSolicitud(int? id)
        {
            try
            {
                var detSolic = db.GS_SRS_PROCEDIMIENTO.Select(
                    x => new
                    {
                        x.IDSRS,
                        x.GS_PROCEDIMIENTO.IDProcedimiento,
                        x.GS_PROCEDIMIENTO.Nombre,
                        x.Estado,
                        x.Ruta
                    });
                return Json(detSolic.ToArray(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public ActionResult VisualizarSolicitud(int? id) {
            GS_SRS gS_SRS = db.GS_SRS.Find(id);
            return View(gS_SRS);
        }


 

        public ActionResult EnviarCorreo(int idsrs, string correo)
        {
            try
            {

                var stream = new WebClient().OpenRead("http://clinica.eastus.cloudapp.azure.com/pdf/" + idsrs + ".pdf");
                var mensaje = new MailMessage();

                mensaje.From = new MailAddress("tpupc@outlook.es");
                mensaje.Subject = "Solicitud de Requerimiento";
                mensaje.Body = "Estimados, adjuntamos archivo de los procedimientos que requieren de su autorización para ser realizados.";
                mensaje.To.Add(new MailAddress(correo));

                mensaje.IsBodyHtml = false;
                mensaje.Attachments.Add(new Attachment(stream, "Códigos_Requerimientos", System.Net.Mime.MediaTypeNames.Application.Pdf));

                var smtp = new SmtpClient();

                smtp.Host = "smtp.office365.com";
                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential("tpupc@outlook.es", "@epenianos");
                smtp.EnableSsl = true;

                smtp.Send(mensaje);

                return Json(null, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex) {
                throw;
            }       
            
        }


    }
}
