using App.Metrics;
using AppMetricsGrafana.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace AppMetricsGrafana.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    private readonly IMetrics _metrics;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IMetrics metrics)
    {
        _logger = logger;
        _metrics = metrics;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _metrics.Measure.Counter.Increment(MetricsRegistry.ForecastGetCounter);
        
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
    }
    
}   