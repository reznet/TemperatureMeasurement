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

            return View(viewModel);
        }
    }
}
