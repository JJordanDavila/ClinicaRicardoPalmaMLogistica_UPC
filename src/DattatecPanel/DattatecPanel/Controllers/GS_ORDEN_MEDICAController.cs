using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DattatecPanel.Models;

namespace DattatecPanel.Controllers
{
    public class GS_ORDEN_MEDICAController : Controller
    {
        private DB_CLINICAEntities db = new DB_CLINICAEntities();

        // GET: GS_ORDEN_MEDICA
        public ActionResult Index()
        {
            var gS_ORDEN_MEDICA = db.GS_ORDEN_MEDICA.Include(g => g.GS_ATENCION);
            return View(gS_ORDEN_MEDICA.ToList());
        }

        // GET: GS_ORDEN_MEDICA/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GS_ORDEN_MEDICA gS_ORDEN_MEDICA = db.GS_ORDEN_MEDICA.Find(id);
            if (gS_ORDEN_MEDICA == null)
            {
                return HttpNotFound();
            }
            return View(gS_ORDEN_MEDICA);
        }

        // GET: GS_ORDEN_MEDICA/Create
        public ActionResult Create()
        {
            ViewBag.IDAtencion = new SelectList(db.GS_ATENCION, "IDAtencion", "Tipo");
            return View();
        }

        // POST: GS_ORDEN_MEDICA/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDOrdenMedica,IDAtencion,Estado,Fecha_registro")] GS_ORDEN_MEDICA gS_ORDEN_MEDICA)
        {
            if (ModelState.IsValid)
            {
                db.GS_ORDEN_MEDICA.Add(gS_ORDEN_MEDICA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDAtencion = new SelectList(db.GS_ATENCION, "IDAtencion", "Tipo", gS_ORDEN_MEDICA.IDAtencion);
            return View(gS_ORDEN_MEDICA);
        }

        // GET: GS_ORDEN_MEDICA/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GS_ORDEN_MEDICA gS_ORDEN_MEDICA = db.GS_ORDEN_MEDICA.Find(id);
            if (gS_ORDEN_MEDICA == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDAtencion = new SelectList(db.GS_ATENCION, "IDAtencion", "Tipo", gS_ORDEN_MEDICA.IDAtencion);
            return View(gS_ORDEN_MEDICA);
        }

        // POST: GS_ORDEN_MEDICA/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDOrdenMedica,IDAtencion,Estado,Fecha_registro")] GS_ORDEN_MEDICA gS_ORDEN_MEDICA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gS_ORDEN_MEDICA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDAtencion = new SelectList(db.GS_ATENCION, "IDAtencion", "Tipo", gS_ORDEN_MEDICA.IDAtencion);
            return View(gS_ORDEN_MEDICA);
        }

        // GET: GS_ORDEN_MEDICA/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GS_ORDEN_MEDICA gS_ORDEN_MEDICA = db.GS_ORDEN_MEDICA.Find(id);
            if (gS_ORDEN_MEDICA == null)
            {
                return HttpNotFound();
            }
            return View(gS_ORDEN_MEDICA);
        }

        // POST: GS_ORDEN_MEDICA/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GS_ORDEN_MEDICA gS_ORDEN_MEDICA = db.GS_ORDEN_MEDICA.Find(id);
            db.GS_ORDEN_MEDICA.Remove(gS_ORDEN_MEDICA);
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

        public JsonResult ShowAllOrden(int? id)
        {
            try
            {
                var ordenList = db.GS_ORDEN_MEDICA.Select(
                    o => new
                    {
                        o.IDOrdenMedica,
                        o.Fecha_registro,
                        o.GS_ATENCION.GS_PACIENTE.IDPaciente,
                        o.GS_ATENCION.GS_PACIENTE.DNI,
                        o.GS_ATENCION.GS_PACIENTE.Nombre,
                        o.GS_ATENCION.GS_MEDICO.IDMedico,
                        o.GS_ATENCION.GS_MEDICO.EMPLEADO.NombreEmp,
                        o.GS_ATENCION.GS_MEDICO.GS_Especialidad.IDEspecialidad,
                        o.GS_ATENCION.GS_MEDICO.GS_Especialidad.NombreEsp,
                        o.Estado
                    });
                ordenList = ordenList.Where(omed => omed.Estado != "S");
                //if(!String.IsNullOrEmpty(id))
                if (id != null)
                {
                    //int ordmedica = Convert.ToInt32(id);
                    ordenList = ordenList.Where(omed => omed.IDOrdenMedica == id);
                }
                else
                {
                    ordenList = ordenList.OrderByDescending(omed => omed.Fecha_registro)
                                         .Take(15);
                }                
                return Json(ordenList.ToArray(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public JsonResult DetOrdenMedica(int? id)
        {
            try
            {
                var detOrden = db.GS_ORDEN_PROCEDIMIENTO.Select(
                    x => new
                    {
                        x.IDOrdenMedica,
                        x.IDProcedimiento,
                        x.GS_PROCEDIMIENTO.Nombre
                    });
                if (id != null)
                {
                    detOrden = detOrden.Where(d => d.IDOrdenMedica == id);
                }
                return Json(detOrden.ToArray(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
