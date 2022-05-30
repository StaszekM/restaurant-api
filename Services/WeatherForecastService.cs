namespace RestaurantApi.Services;

public class WeatherForecastService : IWeatherForecastService
{

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public IEnumerable<WeatherForecast> Get(int numOfResults, int minTemperature, int maxTemperature)
    {
        int range = Math.Abs(maxTemperature - minTemperature);
        return Enumerable.Range(1, numOfResults).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = minTemperature + Random.Shared.Next(range + 1),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
