using DattatecPanel.Context;
using DattatecPanel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Entidades;

namespace DattatecPanel.Controllers
{
    public class PostularController : Controller
    {
        private ClinicaDBContext db = new ClinicaDBContext();

        // GET: Postular
        public ActionResult NuevoPostulante() //int ConvocatoriaID
        {
            PostulanteView postulanteView = new PostulanteView();
            // postulanteView.Convocatoria.Convocatoriaid = 23;
            postulanteView.Postulante = new Postulante();
            postulanteView.DetallePostulantes = new List<DetallePostulante>();
            Session["PostulanteView"] = postulanteView;


            var list = (from a in db.DB_Rubro
                        join b in db.DB_Convocatoria on a.RubroID equals b.RubroID
                        select new { a, b }).ToList();
            ViewBag.RubroID = new SelectList(list, "RubroID", "Descripcion");

            return View(postulanteView);
        }
        [HttpPost]
        public ActionResult NuevoPostulante(PostulanteView postulanteView)
        {
            postulanteView = Session["PostulanteView"] as PostulanteView;
            string razonSocial = Request["Postulante.RazonSocial"];
            string direccion = Request["Postulante.Direccion"];
            string correo = Request["Postulante.Correo"];
            string ruc = Request["Postulante.RUC"];

            Postulante nuevopostulante = new Postulante
            {
                RazonSocial = razonSocial,
                Direccion = direccion,
                Correo = correo,
                RUC = ruc
            };

            db.DB_Postulante.Add(nuevopostulante);
            db.SaveChanges();

            int lastPostulante = db.DB_Postulante.ToList().Select(a => a.PostulanteId).Max();

            int nuevodetalle = 0;
            foreach (DetallePostulante item in postulanteView.DetallePostulantes)
            {
                item.PostulanteId = lastPostulante;
                item.DetalleId = nuevodetalle++;
                // item.NombreArchivo = "def";

                db.DB_DetallePostulante.Add(item);
            }
            db.SaveChanges();

            postulanteView = Session["PostulanteView"] as PostulanteView;

            var list = (from a in db.DB_Rubro
                        join b in db.DB_Convocatoria on a.RubroID equals b.RubroID
                        select new { a, b }).ToList();
            ViewBag.RubroID = new SelectList(list, "RubroID", "Descripcion");

            return View(postulanteView);
        }


        [HttpGet]
        public ActionResult AddDocumento()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddDocumento(DetallePostulante detallepostulante)
        {
            var postulanteview = Session["PostulanteView"] as PostulanteView;
            var detalleId = int.Parse(Request["DetalleId"]);

            detallepostulante = new DetallePostulante
            {
                DetalleId = detalleId
            };

            postulanteview.DetallePostulantes.Add(detallepostulante);

            var list = (from a in db.DB_Rubro
                        join b in db.DB_Convocatoria on a.RubroID equals b.RubroID
                        select new { a, b }).ToList();
            ViewBag.RubroID = new SelectList(list, "RubroID", "Descripcion");

            return View("NuevoPostulante", postulanteview);
        }
    }
}