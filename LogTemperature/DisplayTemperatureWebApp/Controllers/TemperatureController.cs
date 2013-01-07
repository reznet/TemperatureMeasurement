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
            return m_temperatureRepository.GetAll().LastOrDefault();
        }

        public TemperatureTrend GetTrend()
        {
            TemperatureMeasurement nextToLastMeasurement = null;
            TemperatureMeasurement lastMeasurement = null;

            foreach (var measurement in m_temperatureRepository.GetAll())
            {
                nextToLastMeasurement = lastMeasurement;
                lastMeasurement = measurement;
            }

            if (lastMeasurement == null || nextToLastMeasurement == null)
            {
                return TemperatureTrend.Steady;
            }

            double delta = lastMeasurement.TemperatureFahrenheit - nextToLastMeasurement.TemperatureFahrenheit;

            if (delta > 1.0)
            {
                return TemperatureTrend.Increasing;
            }
            else if (delta < -1.0)
            {
                return TemperatureTrend.Decreasing;
            }
            else
            {
                return TemperatureTrend.Steady;
            }
        }
    }
}
