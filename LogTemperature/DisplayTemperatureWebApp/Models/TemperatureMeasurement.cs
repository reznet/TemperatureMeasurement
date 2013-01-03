using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DisplayTemperatureWebApp.Models
{
    public class TemperatureMeasurement
    {
        public int TemperatureMeasurementId { get; set; }

        public double TemperatureCelcius { get; set; }

        public DateTimeOffset MeasurementTimestamp { get; set; }
    }
}