namespace RestaurantApi.Services;

public interface IWeatherForecastService
{
    IEnumerable<WeatherForecast> Get(int numOfResults, int minTemperature, int maxTemperature);
}