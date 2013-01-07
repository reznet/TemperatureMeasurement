using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DisplayTemperatureWebApp.Models
{
    /// <summary>
    /// Indicates whether the temperature is increasing or decreasing.
    /// </summary>
    public enum TemperatureTrend
    {
        /// <summary>
        /// Indicates the temperature is increasing.
        /// </summary>
        Increasing,

        /// <summary>
        /// Indicates the temperature is decreasing.
        /// </summary>
        Decreasing,

        /// <summary>
        /// Indicates the temperature is holding steady.
        /// </summary>
        Steady
    }
}