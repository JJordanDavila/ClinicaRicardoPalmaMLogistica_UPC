using DattatecPanel.Context;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Entidades;
using DattatecPanel.Models.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;

namespace DattatecPanel.Models
{
    public class DetalleConvocatoriaModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ClinicaDBContext db = new ClinicaDBContext();

        public RespuestaJsonDTO RechazarPostulante(RechazarPostulanteDTO datos) {
            try
            {
                RespuestaJsonDTO mensaje = new RespuestaJsonDTO();
                Postulante postulante = db.DB_Postulante.Find(datos.PostulanteId);
                if (postulante != null)
                {
                    using (var dbContextTransaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            //Eliminar archivos relacionados
                            db.Database.ExecuteSqlCommand("Delete From Gl_DetallePostulante Where PostulanteId=@p0", datos.PostulanteId);
                            //Eliminar postulacion
                            db.Database.ExecuteSqlCommand("Delete From GL_DetalleConvocatoria Where ConvocatoriaId=@p0 And PostulanteId=@p1", datos.ConvocatoriaId, datos.PostulanteId);
                            //Eliminar postulante
                            db.Database.ExecuteSqlCommand("Delete From GL_Postulante Where PostulanteId=@p0", datos.PostulanteId);
                            db.SaveChanges();
                            dbContextTransaction.Commit();
                            mensaje.mensaje = "El rechazo fue satisfactorio";
                            mensaje.mensajeInfo = string.Empty;
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            log.Error(ex.Message);
                            throw;
                        }
                    }
                    
                }
                else
                {
                    mensaje.mensaje = string.Empty;
                    mensaje.mensajeInfo = "No se encontró el postulante con identificador " + datos.PostulanteId;
                }
                mensaje.codigo = System.Net.HttpStatusCode.OK;
                return mensaje;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }
        
        public RespuestaJsonDTO ValidarPostulante(ValidarPostulanteDTO datos) {
            try
            {
                RespuestaJsonDTO mensaje = new RespuestaJsonDTO();
                Proveedor proveedor = new Proveedor();
                Postulante postulante = db.DB_Postulante.Find(datos.PostulanteId);
                if(postulante!=null){
                    Proveedor proveedorExistente = db.DB_Proveedor.Where(x => x.RUC == postulante.RUC).FirstOrDefault();
                    if (proveedorExistente == null)
                    {
                        proveedor.ConstanciaRNP = postulante.ConstanciaRNP;
                        proveedor.Correo = postulante.Correo;
                        proveedor.Direccion = postulante.Direccion;
                        proveedor.Estado = "AC";
                        proveedor.RazonSocial = postulante.RazonSocial;
                        proveedor.NombreComercial = postulante.RazonSocial;
                        proveedor.RubroID = datos.RubroID;
                        proveedor.RUC = postulante.RUC;
                        proveedor.PostulanteId = postulante.PostulanteId;
                        db.DB_Proveedor.Add(proveedor);
                        db.SaveChanges();
                        mensaje.mensaje = "Aprobación satisfactoria";
                        mensaje.mensajeInfo = string.Empty;
                    }
                    else{
                        mensaje.mensaje = string.Empty;
                        mensaje.mensajeInfo = "Ya se encuentra registrado un proveedor con el RUC N° " + postulante.RUC;
                    }
                }
                else{
                    mensaje.mensaje = string.Empty;
                    mensaje.mensajeInfo = "No se encontró el postulante con identificador " + datos.PostulanteId;
                }
                mensaje.codigo = System.Net.HttpStatusCode.OK;
                return mensaje;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }

        public List<PostulanteConvocatoriaDTO> ListarDetalleConvocatoriaPostulante(string numeroConvocatoria, string ruc, string razonSocial)
        {
            try
            {
                List<PostulanteConvocatoriaDTO> listaPostulante = new List<PostulanteConvocatoriaDTO>();
                StringBuilder query = new StringBuilder();
                query.AppendLine("Select D.ConvocatoriaId, P.RUC, P.RazonSocial, R.Descripcion, C.Numero, P.PostulanteId, D.Fecha_Registro");
                query.AppendLine("From GL_DetalleConvocatoria D");
                query.AppendLine("Inner Join GL_Postulante P On P.PostulanteId = D.PostulanteId");
                query.AppendLine("Inner Join GL_Convocatoria C on C.ConvocatoriaId = D.ConvocatoriaId");
                query.AppendLine("Inner Join GL_Rubro R on R.RubroID = C.RubroID");
                query.AppendLine("Where C.Estado='E' And P.RUC Not In (Select Ruc From GL_Proveedor);");

                var lista = db.Database.SqlQuery<PostulanteConvocatoriaDTO>(query.ToString()).ToList();
                //foreach (var item in lista)
                //{
                //    listaPostulante.Add(new PostulanteConvocatoriaDTO() 
                //    { 
                //        ConvocatoriaId = item.ConvocatoriaId,
                //        Descripcion = item.Descripcion,
                //        Fecha_Registro = item.Fecha_Registro,
                //        Numero = item.Numero,
                //        PostulanteId = item.PostulanteId,
                //        RazonSocial = item.RazonSocial,
                //        RUC = item.RUC
                //    });
                //}

                /*
                var lista = db.DB_DetalleConvocatoria.Where(x => x.Convocatoria.Numero.Contains(numeroConvocatoria) && x.Postulante.RUC.Contains(ruc) && x.Postulante.RazonSocial.Contains(razonSocial) && x.Convocatoria.Estado == "E" && !db.DB_Proveedor.Any(o => o.RUC == x.Postulante.RUC)).ToList().Select(s => new
               {
                   s.ConvocatoriaId,
                   s.Postulante.RUC,
                   s.Postulante.RazonSocial, 
                   s.Convocatoria.Rubro.Descripcion,
                   s.Convocatoria.Numero,
                   s.PostulanteId,
                   s.Fecha_Registro
               }).ToList();
                 */
                return lista;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }

        public ArchivoDTO DescargarArchivo(int? postulanteId, int? detalleId)
        {
            try
            {
                ArchivoDTO archivo = new ArchivoDTO();

                //datos del postulante
                var archivosBD = db.DB_DetallePostulante.Where(x => x.Postulante.PostulanteId == postulanteId && x.DetalleId == detalleId).ToList().Select(s => new
                {
                    s.DetalleId,
                    s.NombreArchivo,
                    s.Archivo
                }).FirstOrDefault();
                if (archivosBD != null)
                {
                    archivo.Datos = archivosBD.Archivo;
                    archivo.Nombre = archivosBD.NombreArchivo;
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

        public dynamic ObtenerDatosPostulante(int? convocatoriaId, int? postulanteId)
        {
            try
            {
                ValidarPostulanteDTO datos = new ValidarPostulanteDTO();
                
                //datos del postulante
                var detalleConvocatoria = db.DB_DetalleConvocatoria.Where(x => x.ConvocatoriaId == convocatoriaId &&  x.PostulanteId == postulanteId).ToList().Select(s => new
                {
                    s.Postulante.RUC, s.Postulante.RazonSocial, s.Postulante.Correo, s.Convocatoria.Rubro.Descripcion, s.PostulanteId, s.Convocatoria.RubroID, s.Convocatoria.Numero
                }).FirstOrDefault();

                if (detalleConvocatoria!=null)
                {
                    datos.RUC = detalleConvocatoria.RUC;
                    datos.RazonSocial = detalleConvocatoria.RazonSocial;
                    datos.Correo = detalleConvocatoria.Correo;
                    datos.Rubro = detalleConvocatoria.Descripcion;
                    datos.RubroID = detalleConvocatoria.RubroID;
                    datos.PostulanteId = detalleConvocatoria.PostulanteId;
                    datos.NumeroConvocatoria = detalleConvocatoria.Numero;
                }
                //archivos cargados
                var archivosCargados = db.DB_DetallePostulante.Where(x => x.Postulante.PostulanteId == postulanteId).ToList().Select(s => new
                {
                    s.DetalleId, s.NombreArchivo, s.PostulanteId
                }).ToList();
                if (archivosCargados != null)
                {
                    foreach (var item in archivosCargados)
                    {
                        datos.ListaArchivosPresentados.Add(new ArchivosPresentadosDTO() { PostulanteId = item.PostulanteId, Id = item.DetalleId, Nombre = item.NombreArchivo });
                    }
                }

                return datos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }

        public dynamic ObtenerDatosRechazar(int? convocatoriaId, int? postulanteId)
        {
            try
            {
                RechazarPostulanteDTO datos = new RechazarPostulanteDTO();

                //datos del postulante
                var detalleConvocatoria = db.DB_DetalleConvocatoria.Where(x => x.ConvocatoriaId == convocatoriaId && x.PostulanteId == postulanteId).ToList().Select(s => new
                {
                    s.Postulante.RUC,
                    s.Postulante.RazonSocial,
                    s.Postulante.Correo,
                    s.Convocatoria.Rubro.Descripcion,
                    s.PostulanteId,
                    s.Convocatoria.RubroID,
                    s.Convocatoria.Numero,
                    s.Convocatoria.Convocatoriaid
                }).FirstOrDefault();

                if (detalleConvocatoria != null)
                {
                    datos.RUC = detalleConvocatoria.RUC;
                    datos.RazonSocial = detalleConvocatoria.RazonSocial;
                    datos.Correo = detalleConvocatoria.Correo;
                    datos.Rubro = detalleConvocatoria.Descripcion;
                    datos.RubroID = detalleConvocatoria.RubroID;
                    datos.PostulanteId = detalleConvocatoria.PostulanteId;
                    datos.NumeroConvocatoria = detalleConvocatoria.Numero;
                    datos.ConvocatoriaId = detalleConvocatoria.Convocatoriaid;
                }
                return datos;
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw;
            }
        }

    }
}