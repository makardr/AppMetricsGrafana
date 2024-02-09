using System.Diagnostics;
using System.Text;
using App.Metrics;
using AppMetricsGrafana.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace AppMetricsGrafana.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExampleController : ControllerBase
{
    private readonly ILogger<ExampleController> _logger;
    private readonly IMetricsRoot _metrics;
    private readonly Process _process;

    public ExampleController(ILogger<ExampleController> logger, IMetricsRoot metrics)
    {
        _logger = logger;
        _metrics = metrics;
        _process = Process.GetCurrentProcess();
    }

    [HttpGet("echo/{text}")]
    public String Echo(string text)
    {
        _logger.LogInformation("Received message: {message}", text);
        return text;
    }

    [HttpGet("SetGauge")]
    public String SetGauge()
    {
        Random random = new Random();
        int num = random.Next(1, 100);
        _metrics.Measure.Gauge.SetValue(MetricsRegistry.ExampleGaugeOptions, num);
        return "Gauge set to " + num;
    }

    [HttpGet("ResetGauge")]
    public String ResetGauge()
    {
        _metrics.Measure.Gauge.SetValue(MetricsRegistry.ExampleGaugeOptions, 0);
        return "Gauge Reset";
    }

    [HttpGet("AddCounter")]
    public string AddCounter()
    {
        _metrics.Measure.Counter.Increment(MetricsRegistry.ExampleCounterOptions, 1);
        return "Counter added";
    }

    [HttpGet("SubtractCounter")]
    public string SubtractCounter()
    {
        _metrics.Measure.Counter.Increment(MetricsRegistry.ExampleCounterOptions, -1);
        return "Counter subtracted";
    }

    [HttpGet("GetProcessMemory")]
    public long GetProcessMemory()
    {
        var memory = _process.WorkingSet64;
        _metrics.Measure.Gauge.SetValue(MetricsRegistry.ProcessPhysicalMemoryGauge, memory / 1024.0 / 1024.0);
        return memory;
    }

    [HttpGet("OutputMetrics")]
    public async void OutputMetrics()
    {
        var snapshot = _metrics.Snapshot.Get();

        foreach (var formatter in _metrics.OutputMetricsFormatters)
        {
            using (var stream = new MemoryStream())
            {
                await formatter.WriteAsync(stream, snapshot);

                var result = Encoding.UTF8.GetString(stream.ToArray());

                Console.WriteLine(result);
            }
        }
    }

    [HttpGet("MeasureTimer/{milliseconds}")]
    public string MeasureTimer(int milliseconds)
    {
        var tags = new MetricTags(
            new[] { "client_idKey", "routeKey", "testKey3" },
            new[] { "clientIdValue", "routeTemplateValue", "testValue3" }
        );

        using (_metrics.Measure.Timer.Time(MetricsRegistry.ExampleTimerOptions, tags))
        {
            MeasuredMethod(milliseconds);
        }

        return "Request processed after " + milliseconds + "milliseconds";
    }


    [HttpGet("UpdateHistogram/{number}")]
    public string UpdateHistogram(int number)
    {
        _metrics.Measure.Histogram.Update(MetricsRegistry.ExampleHistogramOptions, number);
        return "Histogram observed " + number;
    }

    private static void MeasuredMethod(int milliseconds)
    {
        Thread.Sleep(milliseconds);
    }

    [HttpGet("WriteLog/{number}")]
    public string WriteLog(int number)
    {
        Random rnd = new Random();
        int randomNumber = rnd.Next(1, 100);
        _logger.LogInformation("Logged number is {number} and logged number is {randomNumber}", number, randomNumber);
        return "Log created with logged number " + number + " and random number " + randomNumber;
    }
}