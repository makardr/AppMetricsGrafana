using System.Text;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using AppMetricsGrafana.Metrics;

var builder = WebApplication.CreateBuilder(args);

var metricsBuilder = new MetricsBuilder()
        .Report.ToConsole()
        .OutputMetrics.AsPlainText()
        .OutputMetrics.AsJson()
        .OutputMetrics.AsPrometheusPlainText()
        .OutputMetrics.AsPrometheusProtobuf();

var metrics = metricsBuilder.Build();

builder.Services.AddMetrics(metrics);

builder.WebHost.UseMetricsWebTracking();

//Output to /metrics
builder.WebHost.UseMetrics(options => { options.EndpointOptions = endpointOptions =>
{
    // Output of the metrics text version, protobuff is more efficient
    endpointOptions.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
    endpointOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
    endpointOptions.EnvironmentInfoEndpointEnabled = false;
}; });

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




