using DisplayTemperatureWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DisplayTemperatureWebApp.Controllers
{
    public class HumidityController : ApiController
    {
        // GET api/humidity
        public IEnumerable<HumidityMeasurement> Get()
        {
            return new HumidityMeasurement[0];
        }

        // GET api/humidity/5
        public HumidityMeasurement Get(int id)
        {
            return null;
        }

        // POST api/humidity
        public void Post([FromBody]HumidityMeasurement value)
        {
        }
    }
}
