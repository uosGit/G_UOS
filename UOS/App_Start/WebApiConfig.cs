using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace UOS
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            

            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);


            config.Routes.MapHttpRoute(
            name: "ApiByName",
            routeTemplate: "api/{controller}/{action}/{id}",
             defaults: new { id = RouteParameter.Optional }
            //constraints: new { name = @"^[a-z]+$" }
        );
            

        //    config.Routes.MapHttpRoute(
        //    name: "ApiByAction",
        //    routeTemplate: "api/{controller}/{action}",
        //    defaults: new { action = "Get" }
        //);

            config.Formatters.Remove(config.Formatters.XmlFormatter);
                        
        }

    }
}
