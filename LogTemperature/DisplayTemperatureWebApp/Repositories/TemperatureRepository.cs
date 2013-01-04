﻿using DisplayTemperatureWebApp.Models;
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
                        MeasurementDateTimeUtc = DateTimeOffset.Parse(Convert.ToString(reader["MeasurementTimestamp"])).UtcDateTime
                    };
                }
            }
        }
    }
}