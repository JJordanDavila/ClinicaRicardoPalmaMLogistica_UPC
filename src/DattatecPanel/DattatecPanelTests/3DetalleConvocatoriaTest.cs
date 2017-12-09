using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DattatecPanel.Models.DTO;
using DattatecPanel.Models;

namespace DattatecPanelTests
{
    [TestClass]
    public class _3DetalleConvocatoriaTest
    {
        [TestMethod]
        public void Test_RechazarPostulante()
        {
            RechazarPostulanteDTO entidad = new RechazarPostulanteDTO();

            entidad.PostulanteId = 96;
            entidad.ConvocatoriaId = 5;

            DetalleConvocatoriaModel ndetalleConvocatoriaModel = new DetalleConvocatoriaModel();
            var datos = ndetalleConvocatoriaModel.RechazarPostulante(entidad) as RespuestaJsonDTO;
            Assert.AreEqual(datos.mensaje, "El rechazo fue satisfactorio");
        }

        [TestMethod]
        public void Test_ValidarPostulante()
        {
            ValidarPostulanteDTO entidad = new ValidarPostulanteDTO();

            entidad.PostulanteId = 73;
            entidad.RUC = "10463529774";

            DetalleConvocatoriaModel ndetalleConvocatoriaModel = new DetalleConvocatoriaModel();
            var datos = ndetalleConvocatoriaModel.ValidarPostulante(entidad) as RespuestaJsonDTO;

            if (datos.mensaje.Equals(String.Empty))
            {
                Assert.AreEqual(datos.mensajeInfo, "Ya se encuentra registrado un proveedor con el RUC N° " + entidad.RUC);
            }
            else { Assert.AreEqual(datos.mensaje, "Aprobación satisfactoria"); }

        }


        //[TestMethod]
        //public void Test_ObtenerDatosPostulante()
        //{

        //    int convocatoriaId = 94;
        //    int postulanteId = 5;

        //    DetalleConvocatoriaModel ndetalleConvocatoriaModel = new DetalleConvocatoriaModel();
        //    var datos = ndetalleConvocatoriaModel.ObtenerDatosPostulante(convocatoriaId, postulanteId) as dynamic;                    

        //    Assert.IsNotNull(datos);
        //}
    }
}
