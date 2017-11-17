﻿using DattatecPanel.Context;
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
                var lista = db.Database.SqlQuery<EvaluarProveedorDTO>("PA_Listar_EvaluarProveedores @RUC, @RazonSocial", RUC, RazonSocial).ToList<EvaluarProveedorDTO>();
                return lista;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}