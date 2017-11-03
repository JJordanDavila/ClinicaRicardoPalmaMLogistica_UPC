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
    public class GS_TRAMAController : Controller
    {
        private DB_CLINICAEntities db = new DB_CLINICAEntities();

        // GET: GS_TRAMA
        public ActionResult Index()
        {
            var gS_TRAMA = db.GS_TRAMA.Include(g => g.GS_ASEGURADORA_PERIODO).Include(g => g.GS_TIPO_TRAMA);
            return View(gS_TRAMA.ToList());
        }

        // GET: GS_TRAMA/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GS_TRAMA gS_TRAMA = db.GS_TRAMA.Find(id);
            if (gS_TRAMA == null)
            {
                return HttpNotFound();
            }
            return View(gS_TRAMA);
        }

        // GET: GS_TRAMA/Create
        public ActionResult Create()
        {
            ViewBag.IDAseguradora = new SelectList(db.GS_ASEGURADORA_PERIODO, "IDAseguradora", "Estado");
            ViewBag.IDTipo = new SelectList(db.GS_TIPO_TRAMA, "IDTipo", "Nombre");
            return View();
        }

        // POST: GS_TRAMA/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDTrama,IDTipo,IDAseguradora,IDPeriodo,Ruta,Observacion,Fecha_registro,Usuario")] GS_TRAMA gS_TRAMA)
        {
            if (ModelState.IsValid)
            {
                db.GS_TRAMA.Add(gS_TRAMA);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDAseguradora = new SelectList(db.GS_ASEGURADORA_PERIODO, "IDAseguradora", "Estado", gS_TRAMA.IDAseguradora);
            ViewBag.IDTipo = new SelectList(db.GS_TIPO_TRAMA, "IDTipo", "Nombre", gS_TRAMA.IDTipo);
            return View(gS_TRAMA);
        }

        // GET: GS_TRAMA/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GS_TRAMA gS_TRAMA = db.GS_TRAMA.Find(id);
            if (gS_TRAMA == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDAseguradora = new SelectList(db.GS_ASEGURADORA_PERIODO, "IDAseguradora", "Estado", gS_TRAMA.IDAseguradora);
            ViewBag.IDTipo = new SelectList(db.GS_TIPO_TRAMA, "IDTipo", "Nombre", gS_TRAMA.IDTipo);
            return View(gS_TRAMA);
        }

        // POST: GS_TRAMA/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDTrama,IDTipo,IDAseguradora,IDPeriodo,Ruta,Observacion,Fecha_registro,Usuario")] GS_TRAMA gS_TRAMA)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gS_TRAMA).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDAseguradora = new SelectList(db.GS_ASEGURADORA_PERIODO, "IDAseguradora", "Estado", gS_TRAMA.IDAseguradora);
            ViewBag.IDTipo = new SelectList(db.GS_TIPO_TRAMA, "IDTipo", "Nombre", gS_TRAMA.IDTipo);
            return View(gS_TRAMA);
        }

        // GET: GS_TRAMA/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GS_TRAMA gS_TRAMA = db.GS_TRAMA.Find(id);
            if (gS_TRAMA == null)
            {
                return HttpNotFound();
            }
            return View(gS_TRAMA);
        }

        // POST: GS_TRAMA/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GS_TRAMA gS_TRAMA = db.GS_TRAMA.Find(id);
            db.GS_TRAMA.Remove(gS_TRAMA);
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

        public ActionResult GenerarTrama()
        {
            ViewBag.IDAseguradora = new SelectList(db.GS_ASEGURADORA, "IDAseguradora", "Razon_social");//ViewBag.IDPeriodo = new SelectList(db.GS_PERIODO, "IDPeriodo", "Descripcion");
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult GenerarTrama([Bind(Include = "IDTrama,IDTipo,IDAseguradora,IDPeriodo,Ruta,Observacion,Fecha_registro,Usuario")] GS_TRAMA gS_TRAMA)
        //{
        //    ViewBag.IDAseguradora = new SelectList(db.GS_ASEGURADORA, "IDAseguradora", "Razon_social");
        //    return View();
        //}

        //Función que generar trama a travéz de un Stored Procedure
        public JsonResult generarTramaAjax(int? idperiodo, int? idaseguradora, string usuario, string observacion)
        {
            try
            {
                db.SP_GENERAR_TRAMAS_PDF(idperiodo, idaseguradora, observacion, usuario);
                return Json(null, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Función que traerá a los periodos de las aseguradoras :D
        public JsonResult listaDePeriodos(int? id) {
            try
            {
                return Json(db.listPeriodosByAseguradaId(id).ToArray(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
