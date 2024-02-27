namespace AppMetricsGrafana.Metrics;

public interface IAppTelemetry
{
    void SetGaugeMetrics(int number);

    void ResetGaugeMetrics();

    void AddCounterMetrics();

    void SubtractCounterMetrics();

    long GetProcessMetricsMemory();

    void OutputMetricsMetrics();

    IDisposable MeasureTimerMetrics();

    void UpdateHistogramMetrics(int number);
}