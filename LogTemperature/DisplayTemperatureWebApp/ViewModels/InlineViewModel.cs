using DisplayTemperatureWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DisplayTemperatureWebApp.ViewModels
{
    public class InlineViewModel
    {
        public IEnumerable<LatestTemperatureInfo> LatestTemperatures { get; set; } 
    }
}