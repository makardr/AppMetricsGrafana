using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Gauge;

namespace AppMetricsGrafana.Metrics;

public class MetricsRegistry
{
    public static CounterOptions ForecastGetCounter => new CounterOptions
    {
        Name = "Created Customers",
        Context = "CustomersApi",
        MeasurementUnit = Unit.Calls
    };

    public static GaugeOptions ExampleGaugeOptions => new GaugeOptions
    {
        Name = "ExampleGaugeMetric",
        Context = "ExampleMetricsApi",
        MeasurementUnit = Unit.Items
    };

    public static CounterOptions ExampleCounterOptions => new CounterOptions
    {
        Name = "ExampleCounterMetric",
        Context = "ExampleMetricsApi",
        MeasurementUnit = Unit.Items
    };

    public static GaugeOptions ProcessPhysicalMemoryGauge => new GaugeOptions
    {
        Name = "ExampleMemoryGauge",
        Context = "ExampleMetricsApi",
        MeasurementUnit = Unit.MegaBytes
    };
}