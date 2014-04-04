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
                name: "LatestHumidity",
                routeTemplate: "api/Humidity/Latest",
                defaults: new { controller = "Humidity", action = "GetLatest" }
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
                name: "AllHumidity",
                routeTemplate: "api/Humidity",
                defaults: new { controller = "Humidity", action = "GetAll" }
                );

            config.Routes.MapHttpRoute(
                name: "PostHumidity",
                routeTemplate: "api/Humidity/new",
                defaults: new { controller = "Humidity", action = "Post" }
                );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
