using DattatecPanel.Context;
using DattatecPanel.Models;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Util;
using Microsoft.Reporting.WebForms;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace DattatecPanel.Controllers
{
    public class LicitacionController : Controller
    {
        private ClinicaDBContext db = new ClinicaDBContext();

        public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page, DateTime? fechainicio, DateTime? fechafin)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var licitaciones = from s in db.Licitaciones
                               select s;

            licitaciones = licitaciones.Where(s => !s.Estado.Contains("AN"));
            if (!String.IsNullOrEmpty(searchString))
            {
                licitaciones = licitaciones.Where(s => s.Numero.Contains(searchString));
            }

            if (fechainicio != null && fechafin != null)
            {
                licitaciones = licitaciones.Where(c =>
                                                c.Fecha >= fechainicio &&
                                                c.Fecha <= fechafin);
            }

            licitaciones = licitaciones.OrderBy(s => s.Numero);

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(licitaciones.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult ObtenerRequerimientoCompra()
        {
            var data = db.DB_RequerimientoCompra.Where(a => a.TransaccionCompra.Estado == "PE").OrderBy(a => a.RequerimientoCompraID);
            return View(data.ToList());
        }

        //
        // GET: /Licitacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Licitacion licitacion = db.Licitaciones.Find(id);
            if (licitacion == null)
            {
                return HttpNotFound();
            }
            //licitacion.lstDetalle = db.DB_DetalleTransaccionCompra.Where(c => c.TransaccionCompraID == ordenCompra.OrdenCompraID).ToList();
            return View(licitacion);
        }


        //listado de licitacion con Postores
        public ActionResult Licitacion_Postor(int? id, string modo)
        {
            ViewBag.Licitacion_ID = id;
            ViewBag.Modo = modo;

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Licitacion licitacion = db.Licitaciones.Find(id);
            if (licitacion == null)
            {
                return HttpNotFound();
            }
            licitacion.registroProveedorParticipante_detalle = db.DB_RegistroProveedorParticipante.Where(c => c.LicitacionID == licitacion.LicitacionID).ToList();
            return View(licitacion);
        }

        // cargar los proveedores 
        public ActionResult Proveedores(int? id)
        {
            ViewBag.Licitacion_ID = id;

            var data = db.DB_Proveedor.Where(a => a.Estado == "1").OrderBy(a => a.ProveedorID);

            //var data_proveedor = db.DB_RegistroProveedorParticipante.Where(a => a.Estado == "PE").OrderBy(a => a.ProveedorID);
            //return View(data.ToList());
            return View(data.ToList());
        }

        [HttpPost]
        public ActionResult Registrar_LicitacionProveedor(Proveedor proveedor, int[] licitaciones_id = null, int[] proveedores = null)
        {

            RegistroProveedorParticipante mce_reqgistroProveedor = db.DB_RegistroProveedorParticipante.Find(proveedores[0], licitaciones_id[0]);

            List<RegistroProveedorParticipante> mob_registroProveedor = new List<RegistroProveedorParticipante>();

            if (proveedores != null)
            {
                for (int i = 0; i < proveedores.Length; i++)
                {
                    mce_reqgistroProveedor = new RegistroProveedorParticipante();
                    mce_reqgistroProveedor.LicitacionID = licitaciones_id[i];
                    mce_reqgistroProveedor.ProveedorID = proveedores[i];
                    db.DB_RegistroProveedorParticipante.Add(mce_reqgistroProveedor);
                }

                db.SaveChanges();
                return RedirectToAction("Licitacion_Postor", new { id = licitaciones_id[0], modo = "Agregar" });

            }
            return View();
        }

        //Adjudicar Proveedor
        [HttpPost]
        public ActionResult Adjudicar_Proveedor(int proveedor_id, int Licitacion_id)
        {
            var mce_adjudicar_postor = db.DB_RegistroProveedorParticipante.Find(proveedor_id, Licitacion_id);
            mce_adjudicar_postor.adjudicado = true;

            Licitacion licitacion = db.Licitaciones.Find(Licitacion_id);
            licitacion.Estado = "AJ";

            db.Entry(mce_adjudicar_postor).State = EntityState.Modified;

            db.SaveChanges();


            RegistroProveedorParticipante regproveedor = db.DB_RegistroProveedorParticipante.Where(x => x.adjudicado == true).FirstOrDefault(x => x.LicitacionID == licitacion.LicitacionID);
            RequerimientoCompra nuevo_req = db.DB_RequerimientoCompra.FirstOrDefault(x => x.RequerimientoCompraID == licitacion.RequerimientoCompraID);
            Licitacion nuevo = db.Licitaciones.FirstOrDefault(x => x.LicitacionID == licitacion.LicitacionID);
            MailSMTP correo = new MailSMTP();
            // string bodyProveedor = BodyProveedor(2, Licitaciones.Cotizacion, ordenCompra.lstDetalle, ordenCompraEdit.TransaccionCompra, ordenCompraEdit);
            string bodySolicitante = BodySolicitante_Licitacion(3, nuevo, nuevo_req, regproveedor);
            //  correo.EnviarCorreo("Clínica Ricardo Palma", ordenCompraEdit.Cotizacion.Proveedor.Correo, "Alerta CRP", bodyProveedor, true, null);
            correo.EnviarCorreo("Clínica Ricardo Palma", "gianqarlo@gmail.com", "Alerta CRP", bodySolicitante, true, null);
            return RedirectToAction("Index");


            //return RedirectToAction("Index");
        }

        //Eliminar Proveedor de la Licitacion
        [HttpPost]
        public ActionResult Eliminar_Proveedor_Licitacion(int proveedor_id, int Licitacion_id)
        {
            RegistroProveedorParticipante regproveedor = db.DB_RegistroProveedorParticipante.Find(proveedor_id, Licitacion_id);
            db.DB_RegistroProveedorParticipante.Remove(regproveedor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        #region REGISTRO DE LICITACION

        // GET: /Licitacion/Create
        public ActionResult Create()
        {
            ViewBag.MessageAdvCreateLC = false;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Licitacion licitacion)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (!licitacion.FechaConvocatoria.Equals(null))
                    {
                        if (Convert.ToDateTime(licitacion.FechaConvocatoria) < Convert.ToDateTime(System.DateTime.Now.ToShortDateString()))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria no debe ser menor a la fecha actual";
                            return View(licitacion);
                        }
                    }

                    if (!licitacion.FecRecepcionConsultas.Equals(null))
                    {
                        // debe existir ña fecha convocatoria
                        if (licitacion.FechaConvocatoria.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria debe de existir";
                            return View(licitacion);
                        }
                        //no debe ser menor a la fecha convocatoria
                        if (Convert.ToDateTime(licitacion.FechaConvocatoria) > Convert.ToDateTime(licitacion.FecRecepcionConsultas))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepcoón de Consultas no debe ser menor a la de Convocatoria";
                            return View(licitacion);
                        }
                    }

                    if (!licitacion.FecAbsolucionConsultas.Equals(null))
                    {
                        // debe existir la fecha convocatoria
                        if (licitacion.FechaConvocatoria.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha convocatoria
                        if (licitacion.FecRecepcionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepción de Consultas debe de existir";
                            return View(licitacion);
                        }
                        if (Convert.ToDateTime(licitacion.FecRecepcionConsultas) > Convert.ToDateTime(licitacion.FecAbsolucionConsultas))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Absolución de Consultas no debe ser menor a la fecha Recepción de Consultas";
                            return View(licitacion);
                        }
                    }

                    if (!licitacion.FecRecepcionExpediente.Equals(null))
                    {
                        // debe existir la fecha convocatoria
                        if (licitacion.FechaConvocatoria.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha convocatoria
                        if (licitacion.FecRecepcionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepción de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Absolución de Consultas
                        if (licitacion.FecAbsolucionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Absolución de Consultas debe de existir";
                            return View(licitacion);
                        }
                        if (Convert.ToDateTime(licitacion.FecAbsolucionConsultas) > Convert.ToDateTime(licitacion.FecRecepcionExpediente))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepcoón de Expedientes no debe ser menor a la fecha Absolución de Consultas";
                            return View(licitacion);
                        }
                    }

                    if (!licitacion.FecEvaluacionIni.Equals(null))
                    {
                        // debe existir la fecha convocatoria
                        if (licitacion.FechaConvocatoria.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha convocatoria
                        if (licitacion.FecRecepcionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepción de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Absolución de Consultas
                        if (licitacion.FecAbsolucionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Absolución de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Recepcoón de Expedientes
                        if (licitacion.FecRecepcionExpediente.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepcoón de Expedientes debe de existir";
                            return View(licitacion);
                        }
                        if (Convert.ToDateTime(licitacion.FecRecepcionExpediente) > Convert.ToDateTime(licitacion.FecEvaluacionIni))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Evaluación Inicial no debe ser menor a la fecha Recepcoón de Expedientes";
                            return View(licitacion);
                        }
                    }

                    if (!licitacion.FecEvaluacionFin.Equals(null))
                    {
                        // debe existir la fecha convocatoria
                        if (licitacion.FechaConvocatoria.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha convocatoria
                        if (licitacion.FecRecepcionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepción de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Absolución de Consultas
                        if (licitacion.FecAbsolucionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Absolución de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Recepcoón de Expedientes
                        if (licitacion.FecRecepcionExpediente.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepci&oacute;n de Expedientes debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha de Evaluación Inicial
                        if (licitacion.FecEvaluacionIni.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Evaluaci&oacute;n Inicial debe de existir";
                            return View(licitacion);
                        }
                        if (Convert.ToDateTime(licitacion.FecEvaluacionIni) > Convert.ToDateTime(licitacion.FecEvaluacionFin))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Evaluaci&oacute;n Final no debe ser menor a la fecha de Evaluación Inicial";
                            return View(licitacion);
                        }
                    }

                    if (!licitacion.FecAdjudicacion.Equals(null))
                    {
                        // debe existir la fecha convocatoria
                        if (licitacion.FechaConvocatoria.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha convocatoria
                        if (licitacion.FecRecepcionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepci&oacute;n de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Absolución de Consultas
                        if (licitacion.FecAbsolucionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Absoluci&oacute;n de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Recepcoón de Expedientes
                        if (licitacion.FecRecepcionExpediente.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepci&oacute;n de Expedientes debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha de Evaluación Inicial
                        if (licitacion.FecEvaluacionIni.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Evaluaci&oacute;n Inicial debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Evaluación Final
                        if (licitacion.FecEvaluacionFin.Equals(null))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Evaluaci&oacute;n Final debe de existir";
                            return View(licitacion);
                        }
                        if (Convert.ToDateTime(licitacion.FecEvaluacionFin) > Convert.ToDateTime(licitacion.FecAdjudicacion))
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Adjudicaci&oacute;n no debe ser menor a la fecha Evaluación Final";
                            return View(licitacion);
                        }
                    }

                    //validar la licitacion
                    if (licitacion.FileTDR1 != null)
                    {
                        if (Path.GetExtension(licitacion.FileTDR1.FileName).ToLower() == ".pdf")
                        {
                            byte[] Data = new byte[licitacion.FileTDR1.ContentLength];
                            licitacion.FileTDR1.InputStream.Read(Data, 0, licitacion.FileTDR1.ContentLength);
                            licitacion.TerminoRefInicial = Data;
                            licitacion.ContentTypeTDR1 = licitacion.FileTDR1.ContentType;
                            licitacion.FileNameTDR1 = licitacion.FileTDR1.FileName;
                        }
                        else
                        {
                            ViewBag.MessageAdvCreateLC = true;
                            ViewBag.MessageAdvertenciaLC = "Documento incorrecto, s&oacute;lo se puede subir archivos PDF";
                            return View(licitacion);
                        }
                    }
                    else
                    {
                        ViewBag.MessageAdvCreateLC = true;
                        ViewBag.MessageAdvertenciaLC = "Debe seleccionar un documento";
                        return View(licitacion);
                    }

                    string tipo = "LI";
                    TransaccionCompra transaccionRQ = db.DB_TransaccionCompra.Find(licitacion.RequerimientoCompraID);
                    licitacion.LicitacionID = db.Database.SqlQuery<int>("dbo.Spl_GetIdLic").First();
                    licitacion.Numero = db.Database.SqlQuery<string>("dbo.Spl_GetNumero @p0", tipo).First();
                    licitacion.Fecha = DateTime.Now;
                    licitacion.Estado = "PE";
                    //CAMBIA ESTADO DEL RQ
                    transaccionRQ.Estado = "AT";
                    //TODOS LOS CAMPOS ADICIONALES
                    db.Licitaciones.Add(licitacion);
                    db.Entry(transaccionRQ).State = EntityState.Modified;
                    db.SaveChanges();

                    //ENVIO DE CORREO ELECTRONICO
                    RegistroProveedorParticipante regproveedor = db.DB_RegistroProveedorParticipante.Where(x => x.adjudicado == true).FirstOrDefault(x => x.LicitacionID == licitacion.LicitacionID);
                    RequerimientoCompra nuevo_req = db.DB_RequerimientoCompra.FirstOrDefault(x => x.RequerimientoCompraID == licitacion.RequerimientoCompraID);
                    Licitacion nuevo = db.Licitaciones.FirstOrDefault(x => x.LicitacionID == licitacion.LicitacionID);
                    MailSMTP correo = new MailSMTP();
                    // string bodyProveedor = BodyProveedor(2, Licitaciones.Cotizacion, ordenCompra.lstDetalle, ordenCompraEdit.TransaccionCompra, ordenCompraEdit);
                    string bodySolicitante = BodySolicitante_Licitacion(0, nuevo, nuevo_req, regproveedor);
                    //  correo.EnviarCorreo("Clínica Ricardo Palma", ordenCompraEdit.Cotizacion.Proveedor.Correo, "Alerta CRP", bodyProveedor, true, null);
                    correo.EnviarCorreo("Clínica Ricardo Palma", "gianqarlo@gmail.com", "Alerta CRP", bodySolicitante, true, null);
                    return RedirectToAction("Index");
                }

                return View(licitacion);

            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(licitacion);
        }

        #endregion


        #region MODIFICAR LICITACION
        //
        // GET: /Licitacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Licitacion licitacion = db.Licitaciones.Find(id);
            if (licitacion == null)
            {
                return HttpNotFound();
            }

            licitacion.mob_transaccionCompra = db.DB_TransaccionCompra.Where(c => c.TransaccionCompraID == licitacion.RequerimientoCompraID).ToList();
            ViewBag.MessageAdvEditOC = false;
            return View(licitacion);
        }

        //
        // POST: /Licitacion/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Licitacion licitacion)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    var traer_dato_licitacion = db.Licitaciones.Find(licitacion.LicitacionID);

                    traer_dato_licitacion.FechaConvocatoria = licitacion.FechaConvocatoria;
                    traer_dato_licitacion.FecRecepcionConsultas = licitacion.FecRecepcionConsultas;
                    traer_dato_licitacion.FecAbsolucionConsultas = licitacion.FecAbsolucionConsultas;
                    traer_dato_licitacion.FecRecepcionExpediente = licitacion.FecRecepcionExpediente;
                    traer_dato_licitacion.FecEvaluacionIni = licitacion.FecEvaluacionIni;
                    traer_dato_licitacion.FecEvaluacionFin = licitacion.FecEvaluacionFin;
                    traer_dato_licitacion.FecAdjudicacion = licitacion.FecAdjudicacion;

                    if (!licitacion.FechaConvocatoria.Equals(null))
                    {
                        if (Convert.ToDateTime(licitacion.FechaConvocatoria) < Convert.ToDateTime(System.DateTime.Now.ToShortDateString()))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria no debe ser menor a la fecha actual";
                            return View(licitacion);
                        }
                    }

                    if (!licitacion.FecRecepcionConsultas.Equals(null))
                    {
                        // debe existir ña fecha convocatoria
                        if (licitacion.FechaConvocatoria.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria debe de existir";
                            return View(licitacion);
                        }
                        //no debe ser menor a la fecha convocatoria
                        if (Convert.ToDateTime(licitacion.FechaConvocatoria) > Convert.ToDateTime(licitacion.FecRecepcionConsultas))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepcoón de Consultas no debe ser menor a la de Convocatoria";
                            return View(licitacion);
                        }
                    }

                    if (!licitacion.FecAbsolucionConsultas.Equals(null))
                    {
                        // debe existir la fecha convocatoria
                        if (licitacion.FechaConvocatoria.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha convocatoria
                        if (licitacion.FecRecepcionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepción de Consultas debe de existir";
                            return View(licitacion);
                        }
                        if (Convert.ToDateTime(licitacion.FecRecepcionConsultas) > Convert.ToDateTime(licitacion.FecAbsolucionConsultas))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Absolución de Consultas no debe ser menor a la fecha Recepción de Consultas";
                            return View(licitacion);
                        }
                    }

                    if (!licitacion.FecRecepcionExpediente.Equals(null))
                    {
                        // debe existir la fecha convocatoria
                        if (licitacion.FechaConvocatoria.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha convocatoria
                        if (licitacion.FecRecepcionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepción de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Absolución de Consultas
                        if (licitacion.FecAbsolucionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Absolución de Consultas debe de existir";
                            return View(licitacion);
                        }
                        if (Convert.ToDateTime(licitacion.FecAbsolucionConsultas) > Convert.ToDateTime(licitacion.FecRecepcionExpediente))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepcoón de Expedientes no debe ser menor a la fecha Absolución de Consultas";
                            return View(licitacion);
                        }
                    }

                    if (!licitacion.FecEvaluacionIni.Equals(null))
                    {
                        // debe existir la fecha convocatoria
                        if (licitacion.FechaConvocatoria.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha convocatoria
                        if (licitacion.FecRecepcionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepción de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Absolución de Consultas
                        if (licitacion.FecAbsolucionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Absolución de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Recepcoón de Expedientes
                        if (licitacion.FecRecepcionExpediente.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepcoón de Expedientes debe de existir";
                            return View(licitacion);
                        }
                        if (Convert.ToDateTime(licitacion.FecRecepcionExpediente) > Convert.ToDateTime(licitacion.FecEvaluacionIni))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Evaluación Inicial no debe ser menor a la fecha Recepcoón de Expedientes";
                            return View(licitacion);
                        }
                    }

                    if (!licitacion.FecEvaluacionFin.Equals(null))
                    {
                        // debe existir la fecha convocatoria
                        if (licitacion.FechaConvocatoria.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha convocatoria
                        if (licitacion.FecRecepcionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepción de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Absolución de Consultas
                        if (licitacion.FecAbsolucionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Absolución de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Recepcoón de Expedientes
                        if (licitacion.FecRecepcionExpediente.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepcoón de Expedientes debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha de Evaluación Inicial
                        if (licitacion.FecEvaluacionIni.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Evaluación Inicial debe de existir";
                            return View(licitacion);
                        }
                        if (Convert.ToDateTime(licitacion.FecEvaluacionIni) > Convert.ToDateTime(licitacion.FecEvaluacionFin))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Evaluación Final no debe ser menor a la fecha de Evaluación Inicial";
                            return View(licitacion);
                        }
                    }

                    if (!licitacion.FecAdjudicacion.Equals(null))
                    {
                        // debe existir la fecha convocatoria
                        if (licitacion.FechaConvocatoria.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Convocatoria debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha convocatoria
                        if (licitacion.FecRecepcionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepción de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Absolución de Consultas
                        if (licitacion.FecAbsolucionConsultas.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Absolución de Consultas debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Recepcoón de Expedientes
                        if (licitacion.FecRecepcionExpediente.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Recepcoón de Expedientes debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha de Evaluación Inicial
                        if (licitacion.FecEvaluacionIni.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Evaluación Inicial debe de existir";
                            return View(licitacion);
                        }
                        // debe existir la fecha Evaluación Final
                        if (licitacion.FecEvaluacionFin.Equals(null))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha Evaluación Final debe de existir";
                            return View(licitacion);
                        }
                        if (Convert.ToDateTime(licitacion.FecEvaluacionFin) > Convert.ToDateTime(licitacion.FecAdjudicacion))
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "La fecha de Adjudicación no debe ser menor a la fecha Evaluación Final";
                            return View(licitacion);
                        }
                    }


                    //validar la licitacion
                    if (licitacion.FileTDR2 != null)
                    {
                        if (Path.GetExtension(licitacion.FileTDR2.FileName).ToLower() == ".pdf")
                        {
                            byte[] Data = new byte[licitacion.FileTDR2.ContentLength];
                            licitacion.FileTDR2.InputStream.Read(Data, 0, licitacion.FileTDR2.ContentLength);
                            traer_dato_licitacion.TerminoRefFinal = Data;
                            traer_dato_licitacion.ContentTypeTDR2 = licitacion.FileTDR2.ContentType;
                            traer_dato_licitacion.FileNameTDR2 = licitacion.FileTDR2.FileName;
                        }
                        else
                        {
                            ViewBag.MessageAdvEditLC = true;
                            ViewBag.MessageAdvertenciaLC = "Documento incorrecto, sólo se puede subir archivos PDF";
                            return View(licitacion);
                        }
                    }
                    db.SaveChanges();

                    //ENVIO DE CORREO ELECTRONICO
                    Licitacion nuevo = db.Licitaciones.FirstOrDefault(x => x.LicitacionID == licitacion.LicitacionID);

                    RequerimientoCompra nuevo_req = db.DB_RequerimientoCompra.FirstOrDefault(x => x.RequerimientoCompraID == licitacion.RequerimientoCompraID);

                    RegistroProveedorParticipante regproveedor = db.DB_RegistroProveedorParticipante.Where(x => x.adjudicado == true).FirstOrDefault(x => x.LicitacionID == licitacion.LicitacionID);

                    MailSMTP correo = new MailSMTP();
                    // string bodyProveedor = BodyProveedor(2, Licitaciones.Cotizacion, ordenCompra.lstDetalle, ordenCompraEdit.TransaccionCompra, ordenCompraEdit);
                    string bodySolicitante = BodySolicitante_Licitacion(1, nuevo, nuevo_req, regproveedor);
                    //  correo.EnviarCorreo("Clínica Ricardo Palma", ordenCompraEdit.Cotizacion.Proveedor.Correo, "Alerta CRP", bodyProveedor, true, null);
                    correo.EnviarCorreo("Clínica Ricardo Palma", "gianqarlo@gmail.com", "Alerta CRP", bodySolicitante, true, null);
                    return RedirectToAction("Index");
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        #endregion


        #region ANULAR UNA LICITACION

        //
        // GET: /Licitacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Licitacion licitacion = db.Licitaciones.Find(id);
            if (licitacion == null)
            {
                return HttpNotFound();
            }
            return View(licitacion);

        }

        //
        // POST: /Licitacion/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, Licitacion licitacion)
        {

            var traer_dato_licitacion = db.Licitaciones.FirstOrDefault(x => x.LicitacionID == id);
            traer_dato_licitacion.ObservacionAnulacion = licitacion.ObservacionAnulacion;
            traer_dato_licitacion.Estado = "AN";

            TransaccionCompra transaccionRQ = db.DB_TransaccionCompra.Find(licitacion.RequerimientoCompraID);
            transaccionRQ.Estado = "PE";

            //db.Licitaciones.Add(licitacion);
            db.Entry(transaccionRQ).State = EntityState.Modified;

            db.SaveChanges();


            Licitacion nuevo = db.Licitaciones.FirstOrDefault(x => x.LicitacionID == id);
            RegistroProveedorParticipante regproveedor = db.DB_RegistroProveedorParticipante.Where(x => x.adjudicado == true).FirstOrDefault(x => x.LicitacionID == licitacion.LicitacionID);
            RequerimientoCompra nuevo_req = db.DB_RequerimientoCompra.FirstOrDefault(x => x.RequerimientoCompraID == licitacion.RequerimientoCompraID);
            //ENVIO DE CORREO ELECTRONICO
            MailSMTP correo = new MailSMTP();
            // string bodyProveedor = BodyProveedor(2, Licitaciones.Cotizacion, ordenCompra.lstDetalle, ordenCompraEdit.TransaccionCompra, ordenCompraEdit);
            string bodySolicitante = BodySolicitante_Licitacion(2, nuevo, nuevo_req, regproveedor);
            //  correo.EnviarCorreo("Clínica Ricardo Palma", ordenCompraEdit.Cotizacion.Proveedor.Correo, "Alerta CRP", bodyProveedor, true, null);
            correo.EnviarCorreo("Clínica Ricardo Palma", "gianqarlo@gmail.com", "Alerta CRP", bodySolicitante, true, null);
            return RedirectToAction("Index");
        }

        #endregion

        public FileContentResult FileDownload_TDR1(int id)
        {
            byte[] Data;
            string name;

            var traer_dato_licitacion = db.Licitaciones.FirstOrDefault(x => x.LicitacionID == id);
            Data = (byte[])traer_dato_licitacion.TerminoRefInicial.ToArray();
            name = traer_dato_licitacion.FileNameTDR1;

            return File(Data, "Text", name);
        }

        public FileContentResult FileDownload_TDR2(int id)
        {
            byte[] Data;
            string name;

            var traer_dato_licitacion = db.Licitaciones.FirstOrDefault(x => x.LicitacionID == id);
            Data = (byte[])traer_dato_licitacion.TerminoRefFinal.ToArray();
            name = traer_dato_licitacion.FileNameTDR2;

            return File(Data, "Text", name);
        }

        public ActionResult ObtenerReporte_Licitacion(string id, int? idlic)
        {
            LocalReport lr = new LocalReport();
            string path = Path.Combine(Server.MapPath("~/ReportsRDLC"), "rptDetalleLicitacion.rdlc");
            if (System.IO.File.Exists(path))
            {
                lr.ReportPath = path;
            }
            else
            {
                return View("Index");
            }

            Licitacion licitacion = db.Licitaciones.Find(idlic);
            List<LicitacionDTO> dtLic = new List<LicitacionDTO>();

            LicitacionDTO dtLiCc = new LicitacionDTO
            {
                LicitacionID = licitacion.LicitacionID,
                RequerimientoCompraID = licitacion.RequerimientoCompraID,
                Numero = licitacion.Numero,
                Fecha = Convert.ToDateTime(licitacion.Fecha),
                FechaConvocatoria = Convert.ToDateTime(licitacion.FechaConvocatoria),
                FecRecepcionConsultas = Convert.ToDateTime(licitacion.FecRecepcionConsultas),
                FecAbsolucionConsultas = Convert.ToDateTime(licitacion.FecAbsolucionConsultas),
                FecRecepcionExpediente = Convert.ToDateTime(licitacion.FecRecepcionExpediente),
                FecEvaluacionIni = Convert.ToDateTime(licitacion.FecEvaluacionIni),
                FecEvaluacionFin = Convert.ToDateTime(licitacion.FecEvaluacionFin),
                FecAdjudicacion = Convert.ToDateTime(licitacion.FecAdjudicacion)
            };
            dtLic.Add(dtLiCc);

            licitacion.registroProveedorParticipante_detalle = db.DB_RegistroProveedorParticipante.Where(c => c.LicitacionID == licitacion.LicitacionID).ToList();
            List<LicitacionDetalleDTO> dtLIC_1 = new List<LicitacionDetalleDTO>();

            foreach (var item in licitacion.registroProveedorParticipante_detalle.ToList())
            {
                var doc = new LicitacionDetalleDTO
                {
                    RUC = item.Proveedor.RUC,
                    RazonSocial = item.Proveedor.RazonSocial,
                    Rubro = item.Proveedor.Rubro.Descripcion,
                };
                dtLIC_1.Add(doc);
            }

            ReportDataSource rdLic = new ReportDataSource("dsLicitacion", dtLic);
            ReportDataSource rdLIC = new ReportDataSource("dsLicitacionDetalle", dtLIC_1);
            lr.DataSources.Add(rdLic);
            lr.DataSources.Add(rdLIC);
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

        #region CORREO

        public string BodyProveedor(byte tipo, Licitacion licitacion, Cotizacion cotizacion, List<DetalleTransaccionCompra> lstDetalle, TransaccionCompra transaccion, OrdenCompra ordenCompra)
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

        public string BodySolicitante_Licitacion(int tipo, Licitacion licitacion, RequerimientoCompra reqcompra, RegistroProveedorParticipante regproveedor)
        {
            StringBuilder bodySolicitante = new StringBuilder();
            if (tipo == 0)
            {
                bodySolicitante.Append(string.Format("Estimado {0}:", reqcompra.Solicitante.NombreCompleto));
            }
            else
            {
                bodySolicitante.Append(string.Format("Estimado {0}:", licitacion.RequerimientoCompra.Solicitante.NombreCompleto));
            }
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");
            if (tipo == 0)
            {
                bodySolicitante.Append(string.Format("Se ha generado la Licitacion N° {0} según su Requerimiento de Compra N° {1}", licitacion.Numero, licitacion.RequerimientoCompra.TransaccionCompra.Numero));
            }
            else if (tipo == 1)
            {
                bodySolicitante.Append(string.Format("Se ha modificado la Licitacion N° {0} relacionada a su Requerimiento de Compra N° {1}", licitacion.Numero, licitacion.RequerimientoCompra.TransaccionCompra.Numero));
            }
            else if (tipo == 2)
            {
                bodySolicitante.Append(string.Format("Se ha anulado la Licitacion N° {0} relacionada a su Requerimiento de Compra N° {1}", licitacion.Numero, licitacion.RequerimientoCompra.TransaccionCompra.Numero));
            }
            else
            {
                bodySolicitante.Append(string.Format("Se ha adjudicado la Licitacion N° {0} relacionada a su Requerimiento de Compra N° {1}", licitacion.Numero, licitacion.RequerimientoCompra.TransaccionCompra.Numero));
            }

            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");

            if (tipo == 2)
            {
                bodySolicitante.Append("<b>Motivo de Anulación: </b>" + licitacion.ObservacionAnulacion);
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<br></br>");
            }

            if (tipo == 1)
            {
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<b>Número Licitación: </b>" + licitacion.Numero);
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<b>Número Requerimiento: </b>" + licitacion.RequerimientoCompra.TransaccionCompra.Numero);
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<b>Fecha Convocatoria: </b>" + licitacion.Fecha);
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<b>Fecha Recepción de Consulta: </b>" + licitacion.FecRecepcionConsultas);
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<b>Fecha Absolución de Consulta: </b>" + licitacion.FecAbsolucionConsultas);
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<b>Fecha Recepción de Expedientes: </b>" + licitacion.FecRecepcionExpediente);
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<b>Fecha Evaluación Inicial: </b>" + licitacion.FecEvaluacionIni);
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<b>Fecha Evaluación Final: </b>" + licitacion.FecEvaluacionFin);
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<b>Fecha Adjudicación: </b>" + licitacion.FecAdjudicacion);
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<br></br>");
            }

            if (tipo == 3)
            {
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<b>Postor Adjudicado: </b>" + regproveedor.Proveedor.RazonSocial);
                bodySolicitante.Append("<br></br>");
                bodySolicitante.Append("<br></br>");
            }

            return bodySolicitante.ToString();
        }


        #endregion


        public string BodySolicitante_Licitacion_nuevo(int licitacion_id, int requerimiento_id)
        {

            Licitacion nuevo = db.Licitaciones.FirstOrDefault(x => x.LicitacionID == licitacion_id);

            RequerimientoCompra nuevo_req = db.DB_RequerimientoCompra.FirstOrDefault(x => x.RequerimientoCompraID == requerimiento_id);

            RegistroProveedorParticipante regproveedor = db.DB_RegistroProveedorParticipante.Where(x => x.adjudicado == true).FirstOrDefault(x => x.LicitacionID == licitacion_id);



            StringBuilder bodySolicitante = new StringBuilder();

            bodySolicitante.Append(string.Format("Estimado {0}:", regproveedor.Proveedor.RazonSocial));

            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");

            bodySolicitante.Append(string.Format("La Licitacion N° {0} relacionada a su Requerimiento de Compra N° {1}", nuevo.Numero, nuevo.RequerimientoCompra.TransaccionCompra.Numero));

            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<b>Número Licitación: </b>" + nuevo.Numero);
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<b>Fecha Convocatoria: </b>" + nuevo.FechaConvocatoria);
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<b>Fecha Recepción de Consulta: </b>" + nuevo.FecRecepcionConsultas);
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<b>Fecha Absolución de Consulta: </b>" + nuevo.FecAbsolucionConsultas);
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<b>Fecha Recepción de Expedientes: </b>" + nuevo.FecRecepcionExpediente);
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<b>Fecha Evaluación Inicial: </b>" + nuevo.FecEvaluacionIni);
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<b>Fecha Evaluación Final: </b>" + nuevo.FecEvaluacionFin);
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<b>Fecha Adjudicación: </b>" + nuevo.FecAdjudicacion);
            bodySolicitante.Append("<br></br>");
            bodySolicitante.Append("<br></br>");

            return bodySolicitante.ToString();
        }

        //[HttpPost]
        public ActionResult Notificacion_Registro_Postor(int licitacion_id, int requerimiento_id)
        {
            MailSMTP correo = new MailSMTP();
            // string bodyProveedor = BodyProveedor(2, Licitaciones.Cotizacion, ordenCompra.lstDetalle, ordenCompraEdit.TransaccionCompra, ordenCompraEdit);
            string bodySolicitante = BodySolicitante_Licitacion_nuevo(licitacion_id, requerimiento_id);
            //  correo.EnviarCorreo("Clínica Ricardo Palma", ordenCompraEdit.Cotizacion.Proveedor.Correo, "Alerta CRP", bodyProveedor, true, null);
            correo.EnviarCorreo("Clínica Ricardo Palma", "gianqarlo@gmail.com", "Alerta CRP", bodySolicitante, true, null);
            return RedirectToAction("Index");
        }
    }
}
