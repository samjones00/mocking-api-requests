using Api.Interfaces;
using Api.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using App.Models;

namespace App
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()

               .ConfigureServices((hostingContext, services) =>
               {
                   services.AddSingleton<IWeatherService, WeatherService>();
                   services.AddHttpClient<IWeatherService, WeatherService>();
               })
               .Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var service = serviceScope.ServiceProvider.GetService<IWeatherService>();

                var options = new WeatherServiceRequest
                {
                    Location = "London",
                    BaseUrl = "http://api.openweathermap.org/data/2.5/",
                    AppId = "02ebee3c9a21db50b26de1a5dd0a88a1"
                };

                var result = await service.GetWeatherAsync(options);

                Console.WriteLine($"Location: {result.name}, temp: {result.main.temp}");
            }
        }
    }
}
