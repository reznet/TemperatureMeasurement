using DisplayTemperatureWebApp.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace DisplayTemperatureWebApp.Repositories
{
    public class MeasurementRepository
    {
        public IEnumerable<TemperatureMeasurement> GetAllTemperatures()
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
            var all = GetAllTemperatures();

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
                       Trend = GetTemperatureTrend(last.TemperatureFahrenheit, nextToLast.TemperatureFahrenheit)
                   };
        }

        public void AddHumidityMeasurement(double humidityPercentage, string sourceName)
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.LogHumidityMeasurement";

                command.Parameters.AddWithValue("@HumidityPercentage", humidityPercentage);
                command.Parameters.AddWithValue("@SourceName", sourceName);

                command.ExecuteNonQuery();
            }
        }


        internal IEnumerable<HumidityMeasurement> GetAllHumidity()
        {
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = "dbo.GetHumidityMeasurements";

                var reader = command.ExecuteReader();
                List<HumidityMeasurement> result = new List<HumidityMeasurement>();

                while (reader.Read())
                {
                    result.Add(new HumidityMeasurement()
                    {
                        HumidityMeasurementId = Convert.ToInt32(reader["HumidityMeasurementId"]),
                        HumidityPercentage = Convert.ToDouble(reader["HumidityPercentage"]),
                        MeasurementTimestamp = DateTimeOffset.Parse(Convert.ToString(reader["MeasurementTimestamp"])),
                        MeasurementDateTimeUtc = DateTimeOffset.Parse(Convert.ToString(reader["MeasurementTimestamp"])).UtcDateTime,
                        Source = Convert.ToString(reader["SourceName"])
                    });
                }

                return result;
            }
        }


        public IEnumerable<LatestHumidityInfo> GetLatestHumidityInfos()
        {
            var all = GetAllHumidity();

            var sources = from measurement in all
                          group measurement by measurement.Source into groups
                          select new { SourceName = groups.Key, Measurements = groups };

            return from source in sources
                   let count = source.Measurements.Count()
                   let last = source.Measurements.Last()
                   let nextToLast = source.Measurements.Skip(count - 2).First()
                   select new LatestHumidityInfo
                   {
                       SourceName = source.SourceName,
                       HumidityPercentage = source.Measurements.Last().HumidityPercentage,
                       Trend = GetTemperatureTrend(last.HumidityPercentage, nextToLast.HumidityPercentage)
                   };
        }

        private static MeasurementTrend GetTemperatureTrend(double lastTemperature, double nextToLastTemperature)
        {
            double delta = lastTemperature - nextToLastTemperature;

            if (delta > double.Epsilon)
            {
                return MeasurementTrend.Increasing;
            }
            else if (delta < -double.Epsilon)
            {
                return MeasurementTrend.Decreasing;
            }
            else
            {
                return MeasurementTrend.Steady;
            }
        }
    }
}