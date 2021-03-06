﻿using DattatecPanel.Context;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Entidades;
using DattatecPanel.Models.Util;
using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace DattatecPanel.Models
{
    public class ConvocatoriaModel
    {
        private ClinicaDBContext db = new ClinicaDBContext();
        private MailSMTP correo = new MailSMTP();

        public ListarDTO ListarConvocatoriaProveedores(string numero, string fini, string ffin, int page, int pageSize)
        {
            try
            {
                ListarDTO response = new ListarDTO();

                var dfini = string.IsNullOrEmpty(fini) ? DateTime.MinValue : Convert.ToDateTime(fini);
                var dffin = string.IsNullOrEmpty(ffin) ? DateTime.MaxValue : Convert.ToDateTime(ffin);
                var lista = db.DB_Convocatoria.Where(x => x.Numero.Contains(numero)
               //&& Convert.ToDateTime(x.FechaInicio.ToShortDateString()) >= dfini
               //&& Convert.ToDateTime(x.FechaFin.ToShortDateString()) <= dffin
               && (x.FechaInicio >= dfini
               && x.FechaFin <= dffin)
               && x.Estado == "E").ToList().Select(s => new
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
                throw;
            }
        }

        public string GenerarNumeroCorrelativo()
        {
            var numerogenerado = string.Empty;
            if (db.DB_Convocatoria.Count() > 0)
            {
                var convocatoria = db.DB_Convocatoria.OrderByDescending(x => x.Numero).First();
                if (convocatoria != null)
                {
                    var numero = convocatoria.Numero;
                    var correlativo = Convert.ToInt32(numero.ToString().Substring(6)) + 1;
                    numerogenerado = numero.Substring(0, 6) + correlativo.ToString().PadLeft(6, '0');
                }
            }
            else
            {
                numerogenerado = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + "000001";
            }
            return numerogenerado;
        }

        public ResponseConvocatoria GuardarConvocatoria(ConvocatoriaDTO entidad)
        {
            try
            {
                ResponseConvocatoria response = new ResponseConvocatoria { mensaje = string.Empty, mensajeInfo= string.Empty };
                if (entidad.Convocatoriaid <= 0)
                {
                    if (entidad.RequisitoFile != null)
                    {
                        if (!entidad.RequisitoFile.FileName.ToLower().EndsWith("pdf"))
                        {
                            response.mensajeInfo = "Solo adjuntar archivo en formato PDF.";
                            return response;
                        }
                    }
                    else
                    {
                        response.mensajeInfo = "Adjuntar un archivo.";
                        return response;
                    }
                }
                
                byte[] data = null;
                if (entidad.RequisitoFile != null)
                {
                    using (Stream inputStream = entidad.RequisitoFile.InputStream)
                    {
                        MemoryStream memoryStream = inputStream as MemoryStream;
                        if (memoryStream == null)
                        {
                            memoryStream = new MemoryStream();
                            inputStream.CopyTo(memoryStream);
                        }
                        data = memoryStream.ToArray();
                    }
                }

                //entidad.Estado = "E";
                
                if (entidad.Convocatoriaid <= 0)
                {
                    Convocatoria convocatoria = new Convocatoria
                    {
                        Convocatoriaid = entidad.Convocatoriaid,
                        Numero = GenerarNumeroCorrelativo(),//entidad.Numero,
                        FechaInicio = entidad.FechaInicio,
                        FechaFin = entidad.FechaFin,
                        Estado = "E",
                        RubroID = entidad.RubroID,
                        EmpleadoID = entidad.EmpleadoID,
                        Requisito = data == null ? entidad.Requisito : data
                    };

                    var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == convocatoria.EmpleadoID).FirstOrDefault();
                    var cuerpoCorreo = "Se registro la convocatoria con el numero : " + convocatoria.Numero.ToString();
                    db.DB_Convocatoria.Add(convocatoria);
                    db.SaveChanges();
                    correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Creación de convocatoria", cuerpoCorreo, false, null);
                    response.mensaje = "Se registro con exito";
                }
                else
                {
                    Convocatoria convocatoria = new Convocatoria
                    {
                        Convocatoriaid = entidad.Convocatoriaid,
                        Numero = entidad.Numero,
                        FechaInicio = entidad.FechaInicio,
                        FechaFin = entidad.FechaFin,
                        Estado = "E",
                        RubroID = entidad.RubroID,
                        EmpleadoID = entidad.EmpleadoID,
                        Requisito = data == null ? entidad.Requisito : data
                    };

                    var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == convocatoria.EmpleadoID).FirstOrDefault();
                    var cuerpoCorreo = "Se actualizo la convocatoria con el numero : " + convocatoria.Numero.ToString();
                    db.Entry(convocatoria).State = EntityState.Modified;
                    db.SaveChanges();
                    correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Actualizacion de convocatoria", cuerpoCorreo, false, null);
                    response.mensaje = "Se actualizo con exito";
                }
                return response;
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        public dynamic ListarRubros()
        {
            return db.DB_Rubro.ToList();
        }

        public dynamic ListarEmpleados()
        {
            return db.DB_Empleado.ToList();
        }

        public dynamic ObtenerConvocatoriaPorID(int id)
        {
            return db.DB_Convocatoria.Where(x => x.Convocatoriaid == id).First();
        }

        public ResponseConvocatoria GuardarSuspension(Convocatoria entidad)
        {
            ResponseConvocatoria response = new ResponseConvocatoria();
            if (string.IsNullOrEmpty(entidad.ObservacionSuspension))
            {
                response.mensaje = string.Empty;
                response.mensajeInfo = "Ingrese una observación";
                return response;
            }
            var cuerpoCorreo = "Se suspendio la convocatoria con el número : " + entidad.Numero.ToString();
            var empleado = ObtenerEmpleadoPorID(entidad.EmpleadoID);
            entidad.Estado = "S";
            entidad.FechaSuspension = DateTime.Now;
            db.Entry(entidad).State = EntityState.Modified;
            db.SaveChanges();
            correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Suspension de convocatoria", cuerpoCorreo, false, null);
            response.mensaje = "Se suspendio con exito";
            return response;
        }

        public dynamic ObtenerEmpleadoPorID(int id)
        {
            return db.DB_Empleado.Where(x => x.EmpleadoID == id).FirstOrDefault();
        }

        public string ValidarFiltros(string numero, string fini, string ffin)
        {
            string mensaje = string.Empty;
            long result;
            try
            {
                if (!string.IsNullOrEmpty(numero) && !Int64.TryParse(numero, out result))
                {
                    mensaje = "Solo se admiten numeros en el campo número de convocatoria.";
                }

                if (!string.IsNullOrEmpty(fini) && !string.IsNullOrEmpty(ffin) && string.IsNullOrEmpty(mensaje))
                {
                    if (Convert.ToDateTime(fini) > Convert.ToDateTime(ffin))
                    {
                        mensaje = "La fecha de inicio no puede ser mayor a la fecha fin.";
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = ex.Message;
            }
            return mensaje;
        }
    }
}