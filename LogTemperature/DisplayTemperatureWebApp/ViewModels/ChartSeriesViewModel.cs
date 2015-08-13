using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DisplayTemperatureWebApp.ViewModels
{
    public class ChartSeriesViewModel
    {
        public string Name { get; set; }

        public IEnumerable<ChartValueViewModel> Values { get; set; }
    }
}