using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DattatecPanel.Models;
using DattatecPanel.Models.DTO;
using System.Web;
using System.IO;

namespace DattatecPanelTests
{
    [TestClass]
    public class _1ConvocatoriaTest
    {
        [TestMethod]
        public void Test_GenerarNumeroCorrelativo()
        {
            ConvocatoriaModel nuevaConvocatoriaModel = new ConvocatoriaModel();
            var numeroConvocatoria = nuevaConvocatoriaModel.GenerarNumeroCorrelativo();

            Assert.AreEqual("201711000005", numeroConvocatoria.ToString());
        }

        [TestMethod]
        public void Test_GuardarConvocatoria()
        {
            ConvocatoriaDTO entidad = new ConvocatoriaDTO();

            entidad.Convocatoriaid = 0;

            byte[] bytes = System.IO.File.ReadAllBytes(@"..\Archivo\Ficha_RUC.pdf");
            HttpPostedFileBase objFile = (HttpPostedFileBase)new MemoryPostedFile(bytes, "Ficha_RUC.pdf");

            //byte[] bytes = System.IO.File.ReadAllBytes(@"..\Archivo\Ficha_RUC.docx");
            //HttpPostedFileBase objFile = (HttpPostedFileBase)new MemoryPostedFile(bytes, "Ficha_RUC.docx");

            //datos
            entidad.Numero = "201711000005";
            entidad.FechaInicio = Convert.ToDateTime("05/12/2017");
            entidad.FechaFin = Convert.ToDateTime("05/01/2018");
            entidad.RubroID = 1;
            entidad.EmpleadoID = 2;
            entidad.Requisito = bytes;
            entidad.RequisitoFile = objFile;

            ConvocatoriaModel nuevaConvocatoriaModel = new ConvocatoriaModel();
            var guardar = nuevaConvocatoriaModel.GuardarConvocatoria(entidad) as ResponseConvocatoria;

            Assert.AreEqual("Se registro con exito", guardar.mensaje.ToString());
        }


    }
}
