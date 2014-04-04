using System;
using System.Collections.Generic;
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
            double value;
            var sensor = new WatchportHumiditySensor("COM3", TimeSpan.FromSeconds(5));

            try
            {
                value = sensor.Read();
            }
            catch(TimeoutException timeoutException)
            {
                Console.Error.WriteLine(timeoutException);
                Environment.ExitCode = 1;
                return;
            }

            var sink = new MeasurementSink(new Uri("http://localhost:6390/"), "Computer");

            sink.RecordMeasurement(value).Wait();
        }
    }
}
