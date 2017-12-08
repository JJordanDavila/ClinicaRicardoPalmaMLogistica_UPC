using DattatecPanel.Context;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    public class EvaluarProveedorModel
    {
        private ClinicaDBContext db = new ClinicaDBContext();
        private MailSMTP correo = new MailSMTP();

        public dynamic ListarProveedores(RequestEvaluarProveedor request)
        {
            try
            {
                var RUC = new SqlParameter("@RUC", SqlDbType.VarChar);
                RUC.Value = request.RUC??"";
                var RazonSocial = new SqlParameter("@RazonSocial", SqlDbType.VarChar);
                RazonSocial.Value = request.RazonSocial??"";
                var FechaInicio = new SqlParameter("@FechaInicio", SqlDbType.DateTime);
                FechaInicio.Value = Convert.ToDateTime(request.FechaInicio);
                var FechaFin = new SqlParameter("@FechaFin", SqlDbType.DateTime);
                FechaFin.Value = Convert.ToDateTime(request.FechaFin);
                var lista = db.Database.SqlQuery<EvaluarProveedorDTO>(
                    "PA_Listar_EvaluarProveedores @RUC, @RazonSocial, @FechaInicio, @FechaFin", 
                    RUC, RazonSocial, FechaInicio, FechaFin).ToList<EvaluarProveedorDTO>();
                return lista;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public dynamic ActualizarEstadoProveedor(RequestEvaluarProveedor request)
        {
            try
            {
                var id = new SqlParameter("@ProveedorID", SqlDbType.VarChar);
                id.Value = request.idProveedor;
                var estado = new SqlParameter("@Estado", SqlDbType.VarChar);
                estado.Value = request.Estado;
                var obs = new SqlParameter("@Obs", SqlDbType.VarChar);
                obs.Value = request.Observacion??"";
                var lista = db.Database.SqlQuery<int>("PA_ActualizarEstado_Proveedor @ProveedorID, @Estado, @Obs", id, estado, obs);
                var proveedor = db.DB_Proveedor.Where(x => x.ProveedorID == request.idProveedor).FirstOrDefault();
                correo.EnviarCorreo("Clinica Ricardo Palma", proveedor.Correo, "Suspensión", "Ha sido suspendido en el proceso de evaluación. \nObservacion: "+request.Observacion, true, null);
                return lista;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}