using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Maestro.App_Start
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //if first maphttproute failed
            //ss
            //matches requests for: /api/composite(1,2) : routeTemplate: "api/{controller}/{id1}/{id2}" goto my note
            //matches requests for: /api/composite/1/2 : routeTemplate: "api/{controller}/{id1}/{id2}"

            config.Routes.MapHttpRoute(
                name: "Composite2KeyShort",
                routeTemplate: "api/{controller}/{id1}/{id2}"
            );


            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional } //must be numeric
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApiWithAction",
                routeTemplate: "api/{controller}/{action}"
            );
        }
    }
}