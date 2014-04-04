using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogHumidity
{
    class Program
    {
        static void Main(string[] args)
        {
            string comPortName = ConfigurationManager.AppSettings["ComPortName"];
            if(string.IsNullOrWhiteSpace(comPortName))
            {
                Console.Error.WriteLine("ComPortName must be set in the .config file.");
                Environment.Exit(1);
            }

            string logWebsiteUrlValue = ConfigurationManager.AppSettings["LogWebsiteUri"];
            if (string.IsNullOrWhiteSpace(logWebsiteUrlValue))
            {
                Console.Error.WriteLine("LogWebsiteUri must be set in the .config file.");
                Environment.Exit(1);
            }

            string sourceNameValue = ConfigurationManager.AppSettings["SourceName"];
            if (string.IsNullOrWhiteSpace(sourceNameValue))
            {
                Console.Error.WriteLine("SourceName must be set in the .config file.");
                Environment.Exit(1);
            }

            string readTimeoutSecondsSetting = ConfigurationManager.AppSettings["ReadTimeoutSeconds"];
            int readTimeoutSeconds = 5;
            if(!string.IsNullOrWhiteSpace(readTimeoutSecondsSetting))
            {
                readTimeoutSeconds = int.Parse(readTimeoutSecondsSetting);
            }

            double value = 0;
            var sensor = new WatchportHumiditySensor(comPortName, TimeSpan.FromSeconds(readTimeoutSeconds));

            try
            {
                value = sensor.Read();
            }
            catch(Exception exception)
            {
                Console.Error.WriteLine(exception);
                Environment.Exit(1);
            }

            var sink = new MeasurementSink(new Uri(logWebsiteUrlValue), sourceNameValue);

            sink.RecordMeasurement(value).Wait();
        }
    }
}
