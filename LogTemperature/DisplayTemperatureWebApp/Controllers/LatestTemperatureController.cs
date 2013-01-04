using DisplayTemperatureWebApp.Models;
using DisplayTemperatureWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DisplayTemperatureWebApp.Controllers
{
    public class LatestTemperatureController : ApiController
    {
        public TemperatureMeasurement GetLatestTemperatureMeasurement()
        {
            return (new TemperatureRepository()).GetAll().FirstOrDefault();
        }
    }
}
