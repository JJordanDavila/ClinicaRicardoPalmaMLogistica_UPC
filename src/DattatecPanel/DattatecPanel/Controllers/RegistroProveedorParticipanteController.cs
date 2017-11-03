using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DattatecPanel.Controllers
{
    public class RegistroProveedorParticipanteController : Controller
    {
        //
        // GET: /RegistroProveedorParticipante/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /RegistroProveedorParticipante/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /RegistroProveedorParticipante/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /RegistroProveedorParticipante/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /RegistroProveedorParticipante/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /RegistroProveedorParticipante/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /RegistroProveedorParticipante/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /RegistroProveedorParticipante/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
