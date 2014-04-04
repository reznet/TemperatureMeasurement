using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DisplayTemperatureWebApp.Models
{
    public class HumidityMeasurement
    {
        public int HumidityMeasurementId { get; set; }

        public double HumidityPercentage { get; set; }

        public DateTimeOffset MeasurementTimestamp { get; set; }

        public DateTime MeasurementDateTimeUtc { get; set; }

        public string Source { get; set; }
    }
}