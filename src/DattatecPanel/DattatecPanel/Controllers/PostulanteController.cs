using DattatecPanel.Context;
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

        public ActionResult RegistrarPostulante()
        {
            return View();
        }
    }
}