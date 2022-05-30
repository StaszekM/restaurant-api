using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RestaurantApi.Services;

namespace RestaurantApi.Controllers;


public class Range
{
    [Required]
    public int? min { get; set; }
    [Required]
    public int? max { get; set; }

    public bool isValid()
    {
        return max > min;
    }
}

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IWeatherForecastService _forecast;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherForecastService forecast)
    {
        _logger = logger;
        _forecast = forecast;
    }

    [HttpPost]
    [Route("/generate")]
    public ActionResult<IEnumerable<WeatherForecast>> Get([FromQuery][Required] int numOfResults, [FromBody] Range range)
    {
        if (numOfResults < 1)
        {
            return BadRequest(new { error = "numOfResults is smaller than 1, was " + numOfResults });
        }

        if (!range.isValid())
        {
            return BadRequest(new { error = "Range is invalid, min should be smaller than max" });
        }

        if (range.min == null || range.max == null)
        {
            return BadRequest(new { error = "Min or max were not provided" });
        }

        IEnumerable<WeatherForecast> response = _forecast.Get(numOfResults, range.min.GetValueOrDefault(), range.max.GetValueOrDefault());

        return Ok(response);
    }
}
