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
        private readonly TemperatureRepository m_temperatureRepository;

        public TemperatureController()
        {
            m_temperatureRepository = new TemperatureRepository();
        }

        public IEnumerable<TemperatureMeasurement> GetAll()
        {
            return m_temperatureRepository.GetAll();
        }

        public TemperatureMeasurement GetLatest()
        {
            return m_temperatureRepository.GetAll().FirstOrDefault();
        }
    }
}
