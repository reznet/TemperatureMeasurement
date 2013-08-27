using DisplayTemperatureWebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DisplayTemperatureWebApp.Repositories
{
    public class TemperatureRepository
    {
        public IEnumerable<TemperatureMeasurement> GetAll()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.GetTemperatureMeasurements";

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    yield return new TemperatureMeasurement()
                    {
                        TemperatureMeasurementId = Convert.ToInt32(reader["TemperatureMeasurementId"]),
                        TemperatureCelcius = Convert.ToDouble(reader["TemperatureCelcius"]),
                        TemperatureFahrenheit = Convert.ToDouble(reader["TemperatureFahrenheit"]),
                        MeasurementTimestamp = DateTimeOffset.Parse(Convert.ToString(reader["MeasurementTimestamp"])),
                        MeasurementDateTimeUtc = DateTimeOffset.Parse(Convert.ToString(reader["MeasurementTimestamp"])).UtcDateTime,
                        Source = Convert.ToString(reader["SourceName"])
                    };
                }
            }
        }

        public IEnumerable<LatestTemperatureInfo> GetLatestTemperatureInfos()
        {
            var all = GetAll();

            var sources = from measurement in all
                          group measurement by measurement.Source into groups
                          select new { SourceName = groups.Key, Measurements = groups };

            return from source in sources
                   let count = source.Measurements.Count()
                   let last = source.Measurements.Last()
                   let nextToLast = source.Measurements.Skip(count - 2).First()
                   select new LatestTemperatureInfo 
                   { 
                       SourceName = source.SourceName, 
                       TemperatureFahrenheit = source.Measurements.Last().TemperatureFahrenheit,
                       Trend = GetTrend(last.TemperatureFahrenheit, nextToLast.TemperatureFahrenheit)
                   };
        }

        private static TemperatureTrend GetTrend(double lastTemperature, double nextToLastTemperature)
        {
            double delta = lastTemperature - nextToLastTemperature;

            if (delta > double.Epsilon)
            {
                return TemperatureTrend.Increasing;
            }
            else if (delta < -double.Epsilon)
            {
                return TemperatureTrend.Decreasing;
            }
            else
            {
                return TemperatureTrend.Steady;
            }
        }
    }
}