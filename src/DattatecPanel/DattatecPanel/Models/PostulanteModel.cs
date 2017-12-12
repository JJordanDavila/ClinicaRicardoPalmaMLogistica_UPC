using DattatecPanel.Context;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Entidades;
using DattatecPanel.Models.Util;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace DattatecPanel.Models
{
    public class PostulanteModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ClinicaDBContext db = new ClinicaDBContext();
        private MailSMTP correo = new MailSMTP();
        private int errorCode;
        private string errorMessage;

        public ArchivoDTO DescargarArchivo(int convocatoriaID)
        {
            try
            {
                ArchivoDTO archivo = new ArchivoDTO();

                //datos del postulante
                var convocatoria = db.DB_Convocatoria.Where(x => x.Convocatoriaid == convocatoriaID).ToList().Select(s => new
                {
                    s.Requisito,
                    s.Rubro.Descripcion
                }).FirstOrDefault();
                if (convocatoria != null)
                {
                    archivo.Datos = convocatoria.Requisito;
                    archivo.Nombre = "Requisito_" + convocatoria.Descripcion + ".pdf";
                    archivo.Tipo = "application/pdf";
                }

                return archivo;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }

        public ResponsePostulante GuardarPostulante(PostulanteDTO entidad)
        {
            try
            {
                ResponsePostulante response = new ResponsePostulante { mensaje = string.Empty, mensajeInfo = string.Empty, mensajeDireccion = string.Empty };

                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Postulante postulante = new Postulante
                        {
                            RazonSocial = entidad.RazonSocial,
                            Direccion = entidad.Direccion,
                            Correo = entidad.Correo,
                            RUC = entidad.RUC,
                            ConstanciaRNP = entidad.flagConstanciaRNP,
                            FechaRegistro = DateTime.Now
                        };

                        db.DB_Postulante.Add(postulante);
                        db.SaveChanges();

                        int lastPostulante = db.DB_Postulante.ToList().Select(a => a.PostulanteId).Max();
                        DetalleConvocatoria detalleConvocatoria = new DetalleConvocatoria
                        {
                            PostulanteId = lastPostulante,
                            ConvocatoriaId = entidad.IdConvocatoria,
                            Fecha_Registro = DateTime.Now
                        };

                        db.DB_DetalleConvocatoria.Add(detalleConvocatoria);
                        db.SaveChanges();

                        foreach (var item in entidad.ListaAdjuntos)
                        {
                            var doc = new DetallePostulante
                            {
                                DetalleId = item.Id,
                                PostulanteId = lastPostulante,
                                NombreArchivo = item.Nombre,
                                Archivo = item.Datos
                            };

                            db.DB_DetallePostulante.Add(doc);
                            db.SaveChanges();
                        }
                        dbContextTransaction.Commit();

                        var list = (from a in db.DB_Convocatoria
                                    join b in db.DB_DetalleConvocatoria on a.Convocatoriaid equals b.ConvocatoriaId
                                    select new { a, b }).FirstOrDefault();

                        var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == list.a.EmpleadoID).FirstOrDefault();

                        // enviar correo al empleado
                        var cuerpoCorreoEmpleado = "<html><body>" +
                              "<p><b>Se ha registrado el Postulante: </b>" + entidad.RazonSocial + "</p>" +
                              "<p><b>RUC: </b>" + entidad.RUC + "</p>" +
                              "<p><b>Convocatoria Nro: </b>" + entidad.NumeroConvocatoria + "</p>" +
                              "<p><b>Rubro:  </b>" + list.a.Rubro.Descripcion + "</p>" +
                              "</body> </html> ";
                        correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Registro de Postulante", cuerpoCorreoEmpleado, true, null);


                        // enviar correo al que postuló
                        var cuerpoCorreoPostulante = "<html><body>" +
                            "<p><b>Gracias por postular </b></p> " + entidad.RazonSocial +
                            "<p><b>a la convocatoria Nro: </b>" + entidad.NumeroConvocatoria + "</p>" +
                            "<p><b>Rubro:  </b>" + list.a.Rubro.Descripcion + "</p>" +
                            "</body> </html> ";
                        correo.EnviarCorreo("Clinica Ricardo Palma", entidad.Correo, "Registro de Postulante", cuerpoCorreoPostulante, true, null);

                        response.mensaje = "Se registro con exito";
                        return response;
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                        log.Error(ex.Message);
                        throw;
                    }

                }

            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }

        public ResponsePostulante VerificarRUC(string numeroRUC)
        {
            try
            {
                ResponsePostulante response = new ResponsePostulante { mensaje = string.Empty, mensajeInfo = string.Empty, mensajeDireccion = string.Empty };

                Datos datosSunat = getDatosSunat(numeroRUC);
                if (errorCode == 0)
                {
                    response.mensaje = "1";
                    response.mensajeInfo = datosSunat.razonSocial;
                    response.mensajeDireccion = datosSunat.tipoVia + " " + datosSunat.nombreVia + " " + datosSunat.codigoZona + " " + datosSunat.tipoZona;
                }
                else
                {
                    response.mensaje = "0";
                    response.mensajeInfo = "No existe el RUC ingresado";
                    response.mensajeDireccion = String.Empty;
                }

                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }

        public ResponsePostulante ValidarRuc(string numeroRUC, int convocatoriaId)
        {
            try
            {
                ResponsePostulante response = new ResponsePostulante { mensaje = string.Empty, mensajeInfo = string.Empty };


                var postulanteRUC = (from a in db.DB_Postulante
                                     join b in db.DB_DetalleConvocatoria on a.PostulanteId equals b.PostulanteId
                                     select new { a, b }).Where(x => x.a.RUC.Equals(numeroRUC)).Where(a => a.b.ConvocatoriaId.Equals(convocatoriaId)).ToList();


                if (postulanteRUC.Count() > 0)
                {
                    response.mensaje = "0";
                    response.mensajeInfo = "Usted ya ha postulado a esta convocatoria";
                }
                else
                {
                    Datos datosSunat = getDatosSunat(numeroRUC);
                    if (errorCode == 0)
                    {
                        response.mensaje = "1";
                        response.mensajeInfo = datosSunat.razonSocial;
                    }
                    else
                    {
                        response.mensaje = "0";
                        response.mensajeInfo = "No existe el RUC ingresado";
                    }
                }


                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
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
                errorMessage = "No existe el RUC ingresado";
                return null;
            }
        }

        public ListarDTO ListarConvocatorias(int page, int pageSize)
        {
            try
            {
                ListarDTO response = new ListarDTO();
                var lista = db.DB_Convocatoria.Where(a => a.Estado.Equals("E")).ToList().Select(s => new
                {
                    s.Convocatoriaid,
                    s.Numero,
                    s.FechaInicio,
                    s.FechaFin,
                    s.Estado,
                    s.Rubro.Descripcion,
                    s.Empleado.NombreCompleto
                }).ToList();
                response.total = lista.Count();
                response.lista = lista.Skip((page - 1) * pageSize).Take(pageSize);
                return response;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }

        public dynamic ListarConvocatoriasPorID(int id)
        {
            try
            {
                var lista = db.DB_Convocatoria.Where(x => x.Convocatoriaid == id).Select(s => new { s.Convocatoriaid, s.Numero, s.Rubro }).FirstOrDefault();
                return lista;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }

        public PostulanteDTO MostrarDatosVistaRegistrar(int id)
        {
            try
            {
                var convocatoria = ListarConvocatoriasPorID(id);
                PostulanteDTO postulante = new PostulanteDTO
                {
                    IdConvocatoria = convocatoria.Convocatoriaid,
                    NumeroConvocatoria = convocatoria.Numero,
                    descripcionRubro = convocatoria.Rubro.Descripcion,
                };
                return postulante;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }
    }
}