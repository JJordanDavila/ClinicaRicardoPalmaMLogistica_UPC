using DattatecPanel.Context;
using DattatecPanel.Models;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace DattatecPanel.Controllers
{
    public class RequerimientoCompraController : Controller
    {
        private ClinicaDBContext db = new ClinicaDBContext();

        public ActionResult BuscarRequerimientos()
        {
            var requerimientosPendientes = db.DB_RequerimientoCompra.Where(c => c.TransaccionCompra.Estado == "PE");
            return View(requerimientosPendientes.ToList());
        }

        // GET: RequerimientoCompra
        public ActionResult Index()
        {
            var requerimientoCompras = db.DB_RequerimientoCompra.Include(r => r.Encargado).Include(r => r.Solicitante);
            return View(requerimientoCompras.ToList());
        }

        // GET: RequerimientoCompra/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequerimientoCompra requerimientoCompra = db.DB_RequerimientoCompra.Find(id);
            if (requerimientoCompra == null)
            {
                return HttpNotFound();
            }
            return View(requerimientoCompra);
        }

        // GET: RequerimientoCompra/Create
        public ActionResult Create()
        {
            ViewBag.EncargadoID = new SelectList(db.DB_Empleado, "EmpleadoID", "DNI");
            ViewBag.SolicitanteID = new SelectList(db.DB_Empleado, "EmpleadoID", "DNI");
            return View();
        }

        // POST: RequerimientoCompra/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RequerimientoCompraID,SolicitanteID,EncargadoID,FechaEstimada")] RequerimientoCompra requerimientoCompra)
        {
            if (ModelState.IsValid)
            {
                db.DB_RequerimientoCompra.Add(requerimientoCompra);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EncargadoID = new SelectList(db.DB_Empleado, "EmpleadoID", "DNI", requerimientoCompra.EncargadoID);
            ViewBag.SolicitanteID = new SelectList(db.DB_Empleado, "EmpleadoID", "DNI", requerimientoCompra.SolicitanteID);
            return View(requerimientoCompra);
        }

        // GET: RequerimientoCompra/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequerimientoCompra requerimientoCompra = db.DB_RequerimientoCompra.Find(id);
            if (requerimientoCompra == null)
            {
                return HttpNotFound();
            }
            ViewBag.EncargadoID = new SelectList(db.DB_Empleado, "EmpleadoID", "DNI", requerimientoCompra.EncargadoID);
            ViewBag.SolicitanteID = new SelectList(db.DB_Empleado, "EmpleadoID", "DNI", requerimientoCompra.SolicitanteID);
            return View(requerimientoCompra);
        }

        // POST: RequerimientoCompra/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RequerimientoCompraID,SolicitanteID,EncargadoID,FechaEstimada")] RequerimientoCompra requerimientoCompra)
        {
            if (ModelState.IsValid)
            {
                db.Entry(requerimientoCompra).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EncargadoID = new SelectList(db.DB_Empleado, "EmpleadoID", "DNI", requerimientoCompra.EncargadoID);
            ViewBag.SolicitanteID = new SelectList(db.DB_Empleado, "EmpleadoID", "DNI", requerimientoCompra.SolicitanteID);
            return View(requerimientoCompra);
        }

        // GET: RequerimientoCompra/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RequerimientoCompra requerimientoCompra = db.DB_RequerimientoCompra.Find(id);
            if (requerimientoCompra == null)
            {
                return HttpNotFound();
            }
            return View(requerimientoCompra);
        }

        // POST: RequerimientoCompra/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RequerimientoCompra requerimientoCompra = db.DB_RequerimientoCompra.Find(id);
            db.DB_RequerimientoCompra.Remove(requerimientoCompra);
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