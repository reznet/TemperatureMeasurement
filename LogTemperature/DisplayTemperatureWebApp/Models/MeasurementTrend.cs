using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DisplayTemperatureWebApp.Models
{
    /// <summary>
    /// Indicates whether the measurement is increasing or decreasing.
    /// </summary>
    public enum MeasurementTrend
    {
        /// <summary>
        /// Indicates the measurement is increasing.
        /// </summary>
        Increasing,

        /// <summary>
        /// Indicates the measurement is decreasing.
        /// </summary>
        Decreasing,

        /// <summary>
        /// Indicates the measurement is holding steady.
        /// </summary>
        Steady
    }
}