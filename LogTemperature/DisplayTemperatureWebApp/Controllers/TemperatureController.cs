using DisplayTemperatureWebApp.Models;
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
        private readonly MeasurementRepository m_temperatureRepository;

        public TemperatureController()
        {
            m_temperatureRepository = new MeasurementRepository();
        }

        public IEnumerable<TemperatureMeasurement> GetAll()
        {
            return m_temperatureRepository.GetAllTemperatures();
        }

        public IEnumerable<LatestTemperatureInfo> GetLatest()
        {
            return m_temperatureRepository.GetLatestTemperatureInfos();
        }

        // POST api/humidity
        [HttpPost]
        public void Post([FromBody]TemperatureMeasurement value)
        {
            m_temperatureRepository.AddTemperatureMeasurement(value.TemperatureCelcius, value.Source);
        }
    }
}
