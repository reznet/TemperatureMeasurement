using DisplayTemperatureWebApp.Repositories;
using DisplayTemperatureWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DisplayTemperatureWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly MeasurementRepository m_measurementRepository;

        public HomeController()
        {
            m_measurementRepository = new MeasurementRepository();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inline()
        {
            var viewModel = new InlineViewModel();

            viewModel.LatestTemperatures = m_measurementRepository.GetLatestTemperatureInfos();
            viewModel.LatestHumidities = m_measurementRepository.GetLatestHumidityInfos();
            var temperatures = m_measurementRepository.GetAllTemperatures();

            var g = from temperature in temperatures
                    group temperature by temperature.Source into sources
                    select new ChartSeriesViewModel{ Name = sources.Key, Values = temperatures.Select(t => new ChartValueViewModel{ W = t.MeasurementDateTimeUtc, Value = t.TemperatureFahrenheit })};

            viewModel.Temperatures = g;

            var humidities = m_measurementRepository.GetAllHumidity();

            var g2 = from humidity in humidities
                     group humidity by humidity.Source into sources
                     select new ChartSeriesViewModel { Name = sources.Key + " (Humidity)", Values = humidities.Select(h => new ChartValueViewModel { W = h.MeasurementDateTimeUtc, Value = h.HumidityPercentage }) };

            viewModel.Humidities = g2;

            return View(viewModel);
        }
    }
}
