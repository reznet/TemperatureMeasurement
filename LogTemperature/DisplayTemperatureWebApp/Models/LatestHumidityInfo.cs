using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DisplayTemperatureWebApp.Models
{
    public class LatestHumidityInfo
    {
        public string SourceName { get; set; }

        public double HumidityPercentage { get; set; }

        public MeasurementTrend Trend { get; set; }
    }
}