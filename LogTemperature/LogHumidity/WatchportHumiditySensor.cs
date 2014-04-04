using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LogHumidity
{
    class WatchportHumiditySensor
    {
        private readonly SerialPort _serialPort;

        public WatchportHumiditySensor(string portName, TimeSpan readTimeout)
        {
            _serialPort = new SerialPort(portName);
            _serialPort.NewLine = "\r";
            _serialPort.ReadTimeout = (int)readTimeout.TotalMilliseconds;
        }

        public double Read()
        {
            _serialPort.Open();
            try
            {
                _serialPort.WriteLine("H");

                var result = _serialPort.ReadLine();

                // result will be an integer followed by %
                // e.g. 42%

                if (string.IsNullOrWhiteSpace(result))
                {
                    throw new FormatException(string.Format("Humidity result '{0}' is null or whitespace.", result));
                }

                if (result.Last() != '%')
                {
                    throw new FormatException(string.Format("Humidity result '{0}' does not end in %.", result));
                }

                return Convert.ToDouble(result.Substring(0, result.Length - 1));
            }
            finally
            {
                _serialPort.Close();
            }
        }
    }
}
