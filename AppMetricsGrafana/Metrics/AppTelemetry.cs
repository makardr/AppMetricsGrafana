using System.Diagnostics;
using System.Text;
using App.Metrics;

namespace AppMetricsGrafana.Metrics;

public class AppTelemetry : IAppTelemetry
{
    private readonly IMetricsRoot _metrics;
    private readonly ILogger<AppTelemetry> _logger;
    private readonly Process _process;

    public AppTelemetry(ILogger<AppTelemetry> logger, IMetricsRoot metrics)
    {
        _logger = logger;
        _metrics = metrics;
        _process = Process.GetCurrentProcess();
    }

    public void SetGaugeMetrics(int number)
    {
        _metrics.Measure.Gauge.SetValue(MetricsRegistry.ExampleGaugeOptions, number);
    }

    public void ResetGaugeMetrics()
    {
        _metrics.Measure.Gauge.SetValue(MetricsRegistry.ExampleGaugeOptions, 0);
    }

    public void AddCounterMetrics()
    {
        _metrics.Measure.Counter.Increment(MetricsRegistry.ExampleCounterOptions, 1);
    }

    public void SubtractCounterMetrics()
    {
        _metrics.Measure.Counter.Increment(MetricsRegistry.ExampleCounterOptions, -1);
    }

    public long GetProcessMetricsMemory()
    {
        var memory = _process.WorkingSet64;
        _metrics.Measure.Gauge.SetValue(MetricsRegistry.ProcessPhysicalMemoryGauge, memory / 1024.0 / 1024.0);
        return memory;
    }

    public async void OutputMetricsMetrics()
    {
        var snapshot = _metrics.Snapshot.Get();

        foreach (var formatter in _metrics.OutputMetricsFormatters)
        {
            using (var stream = new MemoryStream())
            {
                await formatter.WriteAsync(stream, snapshot);

                var result = Encoding.UTF8.GetString(stream.ToArray());

                _logger.LogInformation(result);
            }
        }
    }

    public IDisposable MeasureTimerMetrics()
    {
        var tags = new MetricTags(
            new[] { "client_idKey", "routeKey", "testKey3" },
            new[] { "clientIdValue", "routeTemplateValue", "testValue3" }
        );

        return _metrics.Measure.Timer.Time(MetricsRegistry.ExampleTimerOptions, tags);
    }

    public void UpdateHistogramMetrics(int number)
    {
        _metrics.Measure.Histogram.Update(MetricsRegistry.ExampleHistogramOptions, number);
    }
}