using GoIOdotNET;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VSTCoreDefsdotNET;

namespace LogTemperature
{
    class Program
    {
        static IntPtr sensorHandle;

        static int Main(string[] args)
        {
            var options = Args.Configuration.Configure<ConsoleOptions>().CreateAndBind(args);

            if (string.IsNullOrWhiteSpace(options.SourceName))
            {
                Console.WriteLine("SourceName must be provided");
                return 1;
            }

            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];

            GoIO.Init();

            ushort majorVersion;
            ushort minorVersion;

            GoIO.GetDLLVersion(out majorVersion, out minorVersion);
            Console.WriteLine("Using GoIO dll version {0}.{1}", majorVersion, minorVersion);

            GoIO.Diags_SetDebugTraceThreshold(GoIO.TRACE_SEVERITY_LOWEST);

            int numSensors = GoIO.UpdateListOfAvailableDevices(VST_USB_defs.VENDOR_ID, VST_USB_defs.PRODUCT_ID_GO_TEMP);

            StringBuilder buffer = new StringBuilder(GoIO.MAX_SIZE_SENSOR_NAME);
            int status = GoIO.GetNthAvailableDeviceName(buffer, GoIO.MAX_SIZE_SENSOR_NAME, VST_USB_defs.VENDOR_ID, VST_USB_defs.PRODUCT_ID_GO_TEMP, 0);
            if(status != 0)
            {
                Console.WriteLine("Got status {0} when calling GetNthAvailableDeviceName", status);
                return 1;
            }

            // open Go! Temp sensor
            sensorHandle = GoIO.Sensor_Open(buffer.ToString(), VST_USB_defs.VENDOR_ID, VST_USB_defs.PRODUCT_ID_GO_TEMP, 0);
            if(sensorHandle == IntPtr.Zero)
            {
                Console.WriteLine("Could not open sensor.");
                return 1;
            }

            GoIO.Sensor_SetMeasurementPeriod(sensorHandle, 1, GoIO.TIMEOUT_MS_DEFAULT);
            double actualMeasurementPeriodSeconds = GoIO.Sensor_GetMeasurementPeriod(sensorHandle, GoIO.TIMEOUT_MS_DEFAULT);

            int response = GoIO.Sensor_SendCmdAndGetResponse4(sensorHandle, GoIO_ParmBlk.CMD_ID_START_MEASUREMENTS, GoIO.TIMEOUT_MS_DEFAULT);

            // wait for sensor to collect data
            Thread.Sleep(TimeSpan.FromSeconds(1));

            int[] rawMeasurements = new int[10000];
            int measurementCount = GoIO.Sensor_ReadRawMeasurements(sensorHandle, rawMeasurements, 10000);

            if(measurementCount > 0)
            {
                var data = rawMeasurements.Select(m => GoIO.Sensor_ConvertToVoltage(sensorHandle, m)).Select(v => GoIO.Sensor_CalibrateData(sensorHandle, v)).First();
                Console.WriteLine("Got data {0}", data);

                Console.WriteLine("Attempting to log data to SQL");
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand();
                        command.Connection = connection;
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        command.CommandText = "dbo.LogTemperatureMeasurement";
                        command.Parameters.AddWithValue("@TemperatureCelcius", data);
                        command.Parameters.AddWithValue("@SourceName", options.SourceName);
                        command.ExecuteNonQuery();
                    }
                }
                catch (SqlException sqlException)
                {
                    Console.WriteLine("Error making SQL call: " + sqlException.Message);
                    return 1;
                }
            }

            // stop measurements
            GoIO.Sensor_SendCmdAndGetResponse4(sensorHandle, GoIO_ParmBlk.CMD_ID_STOP_MEASUREMENTS, GoIO.TIMEOUT_MS_DEFAULT);

            if(sensorHandle != IntPtr.Zero)
            {
                GoIO.Sensor_Close(sensorHandle);
            }

            return 0;
        }
    }
}
