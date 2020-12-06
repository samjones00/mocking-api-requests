using Api.Interfaces;
using Api.Models;
using App.Models;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;

        public WeatherService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherRoot> GetWeatherAsync(WeatherServiceRequest request)
        {
            _httpClient.BaseAddress = new Uri(request.BaseUrl);

            var response = await _httpClient.GetAsync(request.ResolveUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Api request unsuccessful.");
            }

            return JsonConvert.DeserializeObject<WeatherRoot>(await response.Content.ReadAsStringAsync());
        }
    }
}
