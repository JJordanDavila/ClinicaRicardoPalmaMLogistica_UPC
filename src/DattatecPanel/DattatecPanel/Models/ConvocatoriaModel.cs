using DattatecPanel.Context;
using System;
using System.Linq;

namespace DattatecPanel.Models
{
    public class ConvocatoriaModel
    {
        private ClinicaDBContext db = new ClinicaDBContext();

        public dynamic ListarConvocatoriaProveedores(string numero, string fini, string ffin)
        {
            try
            {
                var dfini = string.IsNullOrEmpty(fini) ? DateTime.MinValue : Convert.ToDateTime(fini);
                var dffin = string.IsNullOrEmpty(ffin) ? DateTime.MaxValue : Convert.ToDateTime(ffin);
                var lista = db.DB_Convocatoria.Where(x => x.Numero.Contains(numero)
               && x.FechaInicio >= dfini
               && x.FechaFin <= dffin
               && x.Estado == "E").ToList().Select(s => new
               {
                   s.Convocatoriaid,
                   s.Numero,
                   s.FechaInicio,
                   s.FechaFin,
                   s.Requisito,
                   s.Estado,
                   s.Rubro.Descripcion,
                   s.Empleado.NombreCompleto
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