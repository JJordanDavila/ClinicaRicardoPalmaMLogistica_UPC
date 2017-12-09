using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DattatecPanel.Models.DTO;
using System.Web;
using DattatecPanel.Models;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace DattatecPanelTests
{
    [TestClass]
    public class _2PostulanteTest
    {
        [TestMethod]
        public void Test_ObtenerRazonSocial()
        {
            PostulanteDTO postulante = null;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://190.117.201.60:89/ServicioSUNAT.Web.Service.DatosSunatRest.svc/api/datosruc?ruc=10463529774");
            req.Method = "GET";
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader reader = new StreamReader(res.GetResponseStream());
            string postulanteJSON = reader.ReadToEnd();
            JavaScriptSerializer js = new JavaScriptSerializer();
            postulante = js.Deserialize<PostulanteDTO>(postulanteJSON);
            Assert.AreEqual("LLANOS ALVARADO MARIO GIANCARLO", postulante.RazonSocial);
        }

        [TestMethod]
        public void Test_ValidarPostulanteConvocatoria()
        {

            var model = new PostulanteModel();
            string numeroRUC = "10463529774";
            int idConvocatoria = 5;

            var result = model.ValidarRuc(numeroRUC, idConvocatoria);

            Assert.AreEqual("0", result.mensaje);
        }


        [TestMethod]
        public void Test_ValidarRegistroPostulante()
        {
            PostulanteDTO entidad = new PostulanteDTO();
            // serializamos todos los campos
            entidad.RazonSocial = "GIANCARLO LLANOS";
            entidad.Direccion = "CALLE LOS NARDOS";
            entidad.Correo = "gianqarlo@gmail.com";
            entidad.RUC = "10463529774";
            entidad.flagConstanciaRNP = false;
            entidad.IdConvocatoria = 5;

            ArchivoDTO nuevo = new ArchivoDTO();

            byte[] bytes = System.IO.File.ReadAllBytes(@"..\Archivo\Ficha_RUC.pdf");
            HttpPostedFileBase objFile = (HttpPostedFileBase)new MemoryPostedFile(bytes, "Ficha_RUC.pdf");

            nuevo.Datos = bytes;
            nuevo.Nombre = "Ficha_RUC.pdf";
            nuevo.Tipo = "pdf";
            entidad.ListaAdjuntos.Add(nuevo);

            //PostulanteController nuevoPostulanteController = new PostulanteController();
            PostulanteModel nuevoPostulanteModel = new PostulanteModel();
            var datos = nuevoPostulanteModel.GuardarPostulante(entidad);
            Assert.AreEqual(datos.mensaje, "Se registro con exito");
        }
    }
}
