using DattatecPanel.Context;
using DattatecPanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DattatecPanel.Controllers
{
    public class PostulanteController : Controller
    {
        private ClinicaDBContext db = new ClinicaDBContext();

        // GET: Postulante
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RegistrarPostulante(Postulante postulante)
        {
            //try
            //{
            //    Postulante postular = new Postulante
            //    {
            //        postular.RazonSocial = postulante.RazonSocial;
            //};
            //}
            //catch (DataException)
            //{
            //    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            //}
            return View();
}
    }
}