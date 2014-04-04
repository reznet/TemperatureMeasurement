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
                var content = new StringContent(string.Format("{{HumidityPercentage={0},Source={1}}}", humidityPercentage, _sourceName));
                var response = await httpClient.PostAsync(_baseUri + "api/humidity", content);

                response.EnsureSuccessStatusCode();
            }
        }
    }
}
