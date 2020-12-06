namespace App.Models
{
    public class WeatherServiceRequest
    {
        public string Location { get; set; }
        public string AppId { get; set; }
        public string BaseUrl { get; set; }
        public string ResolveUrl => $"weather?q={Location}&appid={AppId}&units=metric&mode=json";
    }
}
