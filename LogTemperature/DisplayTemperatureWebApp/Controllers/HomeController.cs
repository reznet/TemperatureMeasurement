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
            var allTemperatures = m_measurementRepository.GetAllTemperatures();
            var allHumidities = m_measurementRepository.GetAllHumidity();

            var temperatureSeries = from temperature in allTemperatures
                    group temperature by temperature.Source into sources
                    select new ChartSeriesViewModel{ Name = sources.Key, Values = allTemperatures.Select(t => new ChartValueViewModel{ MeasurementDateTimeUtc = t.MeasurementDateTimeUtc, Value = t.TemperatureFahrenheit })};

            viewModel.Temperatures = temperatureSeries;

            var humiditySeries = from humidity in allHumidities
                     group humidity by humidity.Source into sources
                     select new ChartSeriesViewModel { Name = sources.Key + " (Humidity)", Values = allHumidities.Select(h => new ChartValueViewModel { MeasurementDateTimeUtc = h.MeasurementDateTimeUtc, Value = h.HumidityPercentage }) };

            viewModel.Humidities = humiditySeries;

            return View(viewModel);
        }
    }
}
