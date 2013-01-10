using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogTemperature
{
    class ConsoleOptions
    {
        [Description("The name of the measurement source (e.g. 'Living Room')")]
        public string SourceName { get; set; }
    }
}
