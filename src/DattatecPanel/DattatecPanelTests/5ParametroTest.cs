using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models;

namespace DattatecPanelTests
{
    [TestClass]
    public class _5ParametroTest
    {
        [TestMethod]
        public void Test_GuardarParametro()
        {
            ParametroDTO entidad = new ParametroDTO();

            entidad.ParametroId = 0;
            entidad.FecIni = Convert.ToDateTime("08/12/2017");
            entidad.FecFin = Convert.ToDateTime("18/12/2017");
            entidad.Intervalo = 1;
            entidad.UnidadMedidaIntervalo = "mm";
            entidad.FecUltPro = Convert.ToDateTime("08/12/2017");
            entidad.UrlServicio01 = "";
            entidad.UrlServicio02 = "";
            entidad.EstadoServicioOSCE = "Activo OSCE";
            entidad.EstadoServicioSUNAT = "Activo SUNAT";

            ParametroModel nuevoParametro = new ParametroModel();
            var datos = nuevoParametro.GuardarParametro(entidad) as ResponseParametro;
            Assert.IsNotNull("Se registro con exito", datos.mensaje);
        }
    }
}
