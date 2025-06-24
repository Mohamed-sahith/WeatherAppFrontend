// WeatherAppFrontend/Models/DTOs/WeatherForecast.cs
namespace WeatherAppFrontend.Models.DTOs
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public double FeelsLike { get; set; }
        public int Humidity { get; set; }
        public int Visibility { get; set; } // In meters
        public double WindSpeed { get; set; }
        public double Pressure { get; set; } // In hPa
        public DateTime Sunrise { get; set; } // Local time
        public DateTime Sunset { get; set; } // Local time
    }
}