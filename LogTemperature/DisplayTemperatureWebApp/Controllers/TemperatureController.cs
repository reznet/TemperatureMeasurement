﻿using DisplayTemperatureWebApp.Models;
using DisplayTemperatureWebApp.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DisplayTemperatureWebApp.Controllers
{
    public class TemperatureController : ApiController
    {
        public IEnumerable<TemperatureMeasurement> GetAll()
        {
            return (new TemperatureRepository()).GetAll();
        }
    }
}
