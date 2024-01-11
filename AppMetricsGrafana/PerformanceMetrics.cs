using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Filtering;
using App.Metrics.Formatters.Json;
using App.Metrics.Formatters.Json.Extensions;

namespace AppMetricsGrafana;

public class PerformanceMetrics
{
    private IMetricsRoot _metrics;
    private CounterOptions _counterOptions;
    
    public PerformanceMetrics()
    {
        var metrics = new MetricsBuilder()
            .OutputMetrics.AsPrometheusPlainText()
            .Build();

        _counterOptions = new CounterOptions { Name = "my_counter" };
    }

    public async void IncreaseCounter()
    {
        // var snapshot = _metrics.Snapshot.Get();
        // var metricData = snapshot.ToMetric();

        _metrics.Measure.Counter.Increment(_counterOptions);

        await Task.WhenAll(_metrics.ReportRunner.RunAllAsync());


        // flush counter
        _metrics.Provider.Counter.Instance(_counterOptions).Reset();
    }


    public void ReportToGrafana()
    {
    }

    public void StartPerformanceMetrics()
    {
    }
}