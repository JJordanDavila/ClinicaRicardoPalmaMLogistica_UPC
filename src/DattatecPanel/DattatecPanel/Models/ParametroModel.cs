using DattatecPanel.Context;
using System;
using System.Linq;

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
    }
}