using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DisplayTemperatureWebApp.Models
{
    public class LatestTemperatureInfo
    {
        public string SourceName { get; set; }

        public double TemperatureFahrenheit { get; set; }
    }
}