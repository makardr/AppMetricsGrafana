using AppMetricsGrafana.Metrics;
using Microsoft.AspNetCore.Mvc;

namespace AppMetricsGrafana.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExampleController : ControllerBase
{
    private readonly IAppTelemetry _telemetry;

    public ExampleController(IAppTelemetry telemetry)
    {
        _telemetry = telemetry;
    }

    [HttpGet("echo/{text}")]
    public string Echo(string text)
    {
        return text;
    }

    [HttpGet("SetGauge/{number}")]
    public string SetGauge(int number)
    {
        _telemetry.SetGaugeMetrics(number);
        return "Gauge set to " + number;
    }

    [HttpGet("ResetGauge")]
    public string ResetGauge()
    {
        _telemetry.ResetGaugeMetrics();
        return "Gauge Reset";
    }

    [HttpGet("AddCounter")]
    public string AddCounter()
    {
        _telemetry.AddCounterMetrics();
        return "Counter added";
    }

    [HttpGet("SubtractCounter")]
    public string SubtractCounter()
    {
        _telemetry.SubtractCounterMetrics();
        return "Counter subtracted";
    }

    [HttpGet("GetProcessMemory")]
    public string GetProcessMemory()
    {
        var memory = _telemetry.GetProcessMetricsMemory() / 1024.0 / 1024.0 ;
        return memory + "MB";
    }

    [HttpGet("OutputMetrics")]
    public void OutputMetrics()
    {
        _telemetry.OutputMetricsMetrics();
    }

    [HttpGet("MeasureTimer/{milliseconds}")]
    public string MeasureTimer(int milliseconds)
    {
        using (_telemetry.MeasureTimerMetrics())
        {
            MeasuredMethod(milliseconds);
        }

        return "Request processed after " + milliseconds + "milliseconds";
    }

    private static void MeasuredMethod(int milliseconds)
    {
        Thread.Sleep(milliseconds);
    }

    [HttpGet("UpdateHistogram/{number}")]
    public string UpdateHistogram(int number)
    {
        _telemetry.UpdateHistogramMetrics(number);
        return "Histogram observed " + number;
    }
}