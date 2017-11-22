using DattatecPanel.Context;
using System;
using System.Linq;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Entidades;
using DattatecPanel.Models.Util;
using System.Data.Entity;
using System.IO;


namespace DattatecPanel.Models
{
    public class ParametroModel
    {
        private ClinicaDBContext db = new ClinicaDBContext();

        public dynamic ListarParametros()
        {
            try
            {
                //var dfini = string.IsNullOrEmpty(fini) ? DateTime.MinValue : Convert.ToDateTime(fini);
                //var dffin = string.IsNullOrEmpty(ffin) ? DateTime.MaxValue : Convert.ToDateTime(ffin);
                var lista = db.DB_Parametro.ToList().Select(s => new
               {
                   s.ParametroId,
                   s.FecIni,
                   s.FecFin,
                   s.Intervalo,
                   s.UnidadMedidaIntervalo,
                   s.FecUltPro,
                   s.UrlServicio01,
                   s.UrlServicio02
               }).ToList();
                return lista;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public ResponseParametro GuardarParametro(ParametroDTO entidad)
        {
            try
            {
                ResponseParametro response = new ResponseParametro { mensaje = string.Empty, mensajeInfo = string.Empty };
                     

                //entidad.Estado = "E";
                Parametro parametro = new Parametro
                {
                    ParametroId = entidad.ParametroId,
                    FecIni = entidad.FecIni,
                    FecFin = entidad.FecFin,
                    Intervalo = entidad.Intervalo,
                    UnidadMedidaIntervalo = entidad.UnidadMedidaIntervalo,
                    FecUltPro = entidad.FecUltPro,
                    UrlServicio01 = entidad.UrlServicio01,
                    UrlServicio02 = entidad.UrlServicio02
                    
                };
                if (entidad.ParametroId <= 0)
                {
                    ////var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == Parametro.EmpleadoID).FirstOrDefault();
                    ////var cuerpoCorreo = "Se registro la Parametro con el numero : " + Parametro.Numero.ToString();
                    db.DB_Parametro.Add(parametro);
                    db.SaveChanges();
                    //correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Creación de Parametro", cuerpoCorreo, false, null);
                    response.mensaje = "Se registro con exito";
                }
                else
                {
                    ////var empleado = db.DB_Empleado.Where(x => x.EmpleadoID == Parametro.EmpleadoID).FirstOrDefault();
                    ////var cuerpoCorreo = "Se actualizo la Parametro con el numero : " + Parametro.Numero.ToString();
                    db.Entry(parametro).State = EntityState.Modified;
                    db.SaveChanges();
                    //correo.EnviarCorreo("Clinica Ricardo Palma", empleado.Correo, "Actualizacion de Parametro", cuerpoCorreo, false, null);
                    response.mensaje = "Se actualizo con exito";
                }
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void CargarCombos()
        {
                
        }

    }
}