using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace DisplayTemperatureWebApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "LatestTemperature",
                routeTemplate: "api/Temperature/Latest",
                defaults: new { controller = "Temperature", action = "GetLatest" }
                );

            config.Routes.MapHttpRoute(
                name: "TemperatureTrend",
                routeTemplate: "api/Temperature/Trend",
                defaults: new { controller = "Temperature", action = "GetTrend" }
                );

            config.Routes.MapHttpRoute(
                name: "AllTemperatures",
                routeTemplate: "api/Temperature",
                defaults: new { controller = "Temperature", action = "GetAll" }
                );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
