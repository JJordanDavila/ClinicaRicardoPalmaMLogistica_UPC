using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DattatecPanel.Controllers;
using DattatecPanel.Models.DTO;
using System.Web.Mvc;

namespace DattatecPanelTests
{
    [TestClass]
    public class _4EvaluarProveedorTest
    {
        [TestMethod]
        public void Test_ListarProveedores()
        {
            RequestEvaluarProveedor request = new RequestEvaluarProveedor();

            request.RUC = "10463529774";
            request.RazonSocial = "";

            EvaluarProveedorController evaluarProveedorController = new EvaluarProveedorController();
            var datos = evaluarProveedorController.ListarProveedores(request) as JsonResult;
            Assert.IsNotNull(datos);
        }
    }
}
