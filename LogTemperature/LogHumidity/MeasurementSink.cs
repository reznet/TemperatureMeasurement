using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LogHumidity
{
    class MeasurementSink
    {
        private readonly Uri _baseUri;
        private readonly string _sourceName;

        public MeasurementSink(Uri baseUri, string sourceName)
        {
            _baseUri = baseUri;
            _sourceName = sourceName;
        }

        public async Task RecordMeasurement(double humidityPercentage)
        {
            using(HttpClient httpClient = new HttpClient())
            {
                var measurement = new { HumidityPercentage = humidityPercentage, Source = _sourceName };
                var response = await httpClient.PostAsJsonAsync(_baseUri + "api/humidity", measurement);

                response.EnsureSuccessStatusCode();
            }
        }
    }
}
