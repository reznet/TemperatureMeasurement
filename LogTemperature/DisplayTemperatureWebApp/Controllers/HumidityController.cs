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
    public class HumidityController : ApiController
    {
        private readonly MeasurementRepository m_measurementRepository;

        public HumidityController()
        {
            m_measurementRepository = new MeasurementRepository();
        }

        // GET api/humidity
        public IEnumerable<HumidityMeasurement> Get()
        {
            return m_measurementRepository.GetAllHumidity();
        }

        // POST api/humidity
        public void Post([FromBody]HumidityMeasurement value)
        {
            m_measurementRepository.AddHumidityMeasurement(value.HumidityPercentage, value.Source);
        }
    }
}
