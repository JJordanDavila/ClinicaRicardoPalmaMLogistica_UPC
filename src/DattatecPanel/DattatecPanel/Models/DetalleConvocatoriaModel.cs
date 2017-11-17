using DattatecPanel.Context;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models.Entidades;
using DattatecPanel.Models.Util;
using System;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace DattatecPanel.Models
{
    public class DetalleConvocatoriaModel
    {
        private ClinicaDBContext db = new ClinicaDBContext();
        private MailSMTP correo = new MailSMTP();

        public dynamic ListarDetalleConvocatoriaPostulante(string ruc, string razonSocial)
        {
            try
            {
                //var dffin = string.IsNullOrEmpty(ffin) ? DateTime.MaxValue : Convert.ToDateTime(ffin);
                var sRuc = string.IsNullOrEmpty(ruc);
                var srazonSocial = string.IsNullOrEmpty(razonSocial);
                var lista = db.DB_DetalleConvocatoria.Where(x =>  x.Postulante.RUC.Contains(ruc)).ToList().Select(s => new
               {
                   s.ConvocatoriaId,
                   s.Postulante.RUC,
                   s.Postulante.RazonSocial, 
                   s.Convocatoria.Rubro.Descripcion,
                   s.Convocatoria.Numero,
                   s.PostulanteId,
                   s.Fecha_Registro
               }).ToList();
                return lista;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}