using System.Text;
using App.Metrics;

var builder = WebApplication.CreateBuilder(args);

var metricsBuilder = new MetricsBuilder()
    .Report.ToConsole()
    .OutputMetrics.AsPlainText()
    .OutputMetrics.AsJson();

var metrics = metricsBuilder.Build();

var snapshot = metrics.Snapshot.Get();

foreach(var formatter in metrics.OutputMetricsFormatters)
{
    using (var stream = new MemoryStream())
    {
        await formatter.WriteAsync(stream, snapshot);

        var result = Encoding.UTF8.GetString(stream.ToArray());

        Console.WriteLine(result);
    }
}


builder.Services.AddMetrics(metrics);

builder.WebHost.UseMetricsWebTracking();
// builder.WebHost.UseMetrics(options => { options.EndpointOptions = endpointOptions =>
// {
//     // Output of the metrics text version, protobuff is more efficient
//     endpointOptions.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
//     endpointOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
//     endpointOptions.EnvironmentInfoEndpointEnabled = false;
// }; });

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// AddMetrics
builder.Services.AddMetrics();


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();



// var filter = new MetricsFilter().WhereType(MetricType.Counter);

// var metrics = new MetricsBuilder()
//     .Report.ToConsole()
//     .Report.ToHostedMetrics(
//         options => {
//             options.HostedMetrics.BaseUri = new Uri("https://graphite-prod-24-prod-eu-west-2.grafana.net/graphite");
//             options.HostedMetrics.ApiKey = "1361264:glc_eyJvIjoiMTAyNDMzOSIsIm4iOiJzdGFjay04MjU5MDEtaG0tcmVhZC1yZWFkLXRva2VuIiwiayI6IjVnSVVBZjg0WHNtNXA0MVlaMTJIczI2ZiIsIm0iOnsiciI6InByb2QtZXUtd2VzdC0yIn19";
//             options.HttpPolicy.BackoffPeriod = TimeSpan.FromSeconds(30);
//             options.HttpPolicy.FailuresBeforeBackoff = 5;
//             options.HttpPolicy.Timeout = TimeSpan.FromSeconds(10);
//             // options.Filter = filter;
//             options.FlushInterval = TimeSpan.FromSeconds(10);
//         })
//     .OutputMetrics.AsPlainText()
//     .Build();

// var metrics = new MetricsBuilder()
//     .Report.ToConsole()
//     .Build();


// builder.Services.AddSingleton<IMetrics>(metrics);

// builder.Services.AddAppMetricsGcEventsMetricsCollector();
// builder.Services.AddAppMetricsSystemMetricsCollector();
// builder.Services.AddAppMetricsCollectors();
