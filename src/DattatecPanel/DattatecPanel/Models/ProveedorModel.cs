using DattatecPanel.Context;
using DattatecPanel.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DattatecPanel.Models
{
    public class ProveedorModel
    {
        private ClinicaDBContext db = new ClinicaDBContext();

        public dynamic HistorialProveedor(int? proveedorId)
        {
            try
            {

                HistorialProveedorDTO historial = new HistorialProveedorDTO();

                var proveedor = db.DB_Proveedor.Where(x => x.ProveedorID==proveedorId).FirstOrDefault();

                historial.Ruc = proveedor.RUC;
                historial.RazonSocial = proveedor.RazonSocial;
                switch (proveedor.Estado.ToUpper()) {
                    case "AC": historial.Estado = "ACTIVO"; break;
                    case "SU": historial.Estado = "SUSPENDIDO"; break;
                }
                historial.ObservacionesSuspension = proveedor.ObservacionesSuspension;
                
                var listaLicitacion = db.DB_RegistroProveedorParticipante.Where(x => x.ProveedorID == proveedorId).ToList().Select(s => new
                {
                    s.Licitacion.Numero,
                    s.Licitacion.Fecha,
                    s.fechaColExpediente,
                    s.monto,
                }).ToList();

                if (listaLicitacion != null && listaLicitacion.Count > 0) {
                    foreach (var item in listaLicitacion)
                    {
                        historial.ListaParticipacionLicitacionDTO.Add(
                            new ParticipacionLicitacionDTO() { Fecha = (DateTime)item.Fecha, Numero = item.Numero, FechaColocacionExpediente= item.fechaColExpediente, Moneda = "Soles", Monto = item.monto }
                        );
                    }
                }

                var listaCotizacion = db.DB_Cotizacion.Where(x => x.ProveedorID == proveedorId).ToList().Select(s => new
                {
                    s.TransaccionCompra.Numero,
                    s.TransaccionCompra.Fecha,
                    s.FormaPago.Nombre,
                    s.FormaPago.NroDiasPago
                }).ToList();

                if (listaCotizacion != null && listaCotizacion.Count > 0)
                {
                    foreach (var item in listaCotizacion)
                    {
                        historial.ListaParticipacionCotizacionDTO.Add(
                            new ParticipacionCotizacionDTO() { Fecha = (DateTime)item.Fecha, Numero = item.Numero, FormaPago = item.Nombre, NroDiasPago = item.NroDiasPago  }
                        );
                    }
                }

                return historial;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public dynamic ListarProveedor(string ruc, string razonSocial)
        {
            try
            {
                var lista = db.DB_Proveedor.Where(x => x.RUC.Contains(ruc) && x.RazonSocial.Contains(razonSocial)).ToList().Select(s => new
               {
                   s.ProveedorID,
                   s.RazonSocial,
                   s.RUC,
                   s.ObservacionesSuspension,
                   s.Estado,
                   s.CertificadoISO,
                   s.ConstanciaRNP
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