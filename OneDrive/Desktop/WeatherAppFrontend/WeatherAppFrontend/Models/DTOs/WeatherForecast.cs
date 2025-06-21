namespace WeatherAppFrontend.Models.DTOs;

public class WeatherForecast
{
    public DateTime Date { get; set; }
    public double Temperature { get; set; }
    public double FeelsLike { get; set; }
    public string Summary { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public int Humidity { get; set; } // Change to int to match backend
    public double WindSpeed { get; set; }
    public string Name { get; set; } = string.Empty; // City name
    public double Visibility { get; set; } // Visibility in meters
}