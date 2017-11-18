using DattatecPanel.Context;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Entidades;
using DattatecPanel.Models.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace DattatecPanel.Models
{
    public class PostulanteModel
    {
        private ClinicaDBContext db = new ClinicaDBContext();
        private MailSMTP correo = new MailSMTP();
        private int errorCode;
        private string errorMessage;

        public ResponsePostulante GuardarPostulante(Postulante entidad, int id)
        {
            try
            {
                ResponsePostulante response = new ResponsePostulante { mensaje = string.Empty, mensajeInfo = string.Empty };


                //Datos datosSunat = getDatosSunat(entidad.RUC);
                //if (errorCode == 0)
                //{
                // response.mensaje = datosSunat.razonSocial;

                Postulante postulante = new Postulante
                {
                    PostulanteId = entidad.PostulanteId,
                    RazonSocial = entidad.RazonSocial,
                    Direccion = entidad.Direccion,
                    Correo = entidad.Correo,
                    RUC = entidad.RUC,
                    ConstanciaRNP = entidad.ConstanciaRNP
                };

                db.DB_Postulante.Add(postulante);
                db.SaveChanges();

                int lastPostulante = db.DB_Postulante.ToList().Select(a => a.PostulanteId).Max();
                //entidad.Convocatoria.Convocatoriaid
                DetalleConvocatoria detalleConvocatoria = new DetalleConvocatoria
                {
                    PostulanteId = lastPostulante,
                    ConvocatoriaId = 5
                };

                db.DB_DetalleConvocatoria.Add(detalleConvocatoria);
                db.SaveChanges();

                foreach (var item in entidad.DetallePostulantes)
                {
                    var doc = new DetallePostulante
                    {
                        DetalleId = item.DetalleId,
                        PostulanteId = lastPostulante,
                        NombreArchivo = item.NombreArchivo,
                        Archivo = item.Archivo
                    };

                    db.DB_DetallePostulante.Add(doc);
                }

                //if (entidad.DetallePostulantes.Count > 0)
                //{
                db.SaveChanges();

                response.mensaje = "Se registro con exito";
                //}


                //if (entidad.Convocatoriaid <= 0)
                //{
                //  var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == convocatoria.EmpleadoID).FirstOrDefault();
                var cuerpoCorreo = "Se registro el postulante";
                //    db.DB_Convocatoria.Add(convocatoria);
                //    db.SaveChanges();
                // correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Creación de convocatoria", cuerpoCorreo, false, null);
                //    response.mensaje = "Se registro con exito";
                //}
                //else
                //{
                //    var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == convocatoria.EmpleadoID).FirstOrDefault();
                //    var cuerpoCorreo = "Se actualizo la convocatoria con el numero : " + convocatoria.Numero.ToString();
                //    db.Entry(convocatoria).State = EntityState.Modified;
                //    db.SaveChanges();
                //    correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Actualizacion de convocatoria", cuerpoCorreo, false, null);
                //    response.mensaje = "Se actualizo con exito";
                //}
                //}
                //else
                //{
                //    response.mensaje = errorMessage;
                //}

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public Datos getDatosSunat(string ruc)
        {
            try
            {
                Datos datosSunat = null;
                using (WebClient client = new WebClient())
                {
                    string url = "http://190.117.201.60:89/ServicioSUNAT.Web.Service.DatosSunatRest.svc/api/datosruc" + "?ruc=" + ruc;
                    string json = Encoding.UTF8.GetString(client.DownloadData(url));
                    datosSunat = new JavaScriptSerializer().Deserialize<Datos>(json);
                }
                return datosSunat;
            }
            catch (WebException ex)
            {
                errorCode = (int)((HttpWebResponse)ex.Response).StatusCode;
                errorMessage = (new JavaScriptSerializer().Deserialize<ErrorMessage>(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd())).Mensaje;
                return null;
            }
            catch (Exception ex)
            {
                errorCode = -1;
                errorMessage = ex.Message;
                return null;
            }
        }
    }
}