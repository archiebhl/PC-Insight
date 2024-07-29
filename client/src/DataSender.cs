using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace client
{
    public class DataSender
    {
        private static HttpClient httpClient = new HttpClient();
        private string serverUrl = "localhost"; 

        public async Task SendDataAsync(
            List<double> cpuUsageData,
            List<double> cpuTemperatureData,
            List<double> gpuUsageData,
            List<double> gpuTemperatureData,
            Guid clientId)
        {
            var data = new
            {
                ClientId = clientId,
                CPUUsageData = cpuUsageData,
                CPUTemperatureData = cpuTemperatureData,
                GPUUsageData = gpuUsageData,
                GPUTemperatureData = gpuTemperatureData
            };

            string jsonData = JsonConvert.SerializeObject(data);

            HttpContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await httpClient.PostAsync(serverUrl, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Server response: {responseBody}");
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }
    }
}
