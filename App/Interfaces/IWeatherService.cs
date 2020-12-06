using Api.Models;
using App.Models;
using System.Threading.Tasks;

namespace Api.Interfaces
{
    public interface IWeatherService
    {
        Task<WeatherRoot> GetWeatherAsync(WeatherServiceRequest request);
    }
}
