using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DattatecPanel
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ValidarPostulante",
                url: "{controller}/{action}/{convocatoriaId}/{postulanteId}",
                defaults: new { controller = "DetalleConvocatoria", action = "Validar", id = UrlParameter.Optional, idx = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}/{idx}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, idx = UrlParameter.Optional }
            );

            routes.MapRoute(
            name: "Hello",
            url: "{controller}/{action}/{name}/{id}"
            );

            routes.MapRoute(
                name: "GenerarTrama",
                url: "{controller}/{action}/{idperiodo}/{idaseguradora}/{usuario}/{observacion}",
                defaults: new { controller = "GS_TRAMA", action = "generarTramaAjax", idperiodo = UrlParameter.Optional, idaseguradora = UrlParameter.Optional, usuario = "Pepito Perez", observacion = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "EnviarCorreo",
                url: "{controller}/{action}/{idsrs}/{correo}",
                defaults: new { controller = "GS_SRS", action = "EnviarCorreo", idsrs = UrlParameter.Optional, correo = UrlParameter.Optional }
            );

        }
    }
}
