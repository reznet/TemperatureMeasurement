using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DisplayTemperatureWebApp.ViewModels
{
    public class ChartValueViewModel
    {
        public DateTime MeasurementDateTimeUtc { get; set; }
        public double Value { get; set; }
    }
}