
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;

var builder = WebApplication.CreateBuilder(args);


//Metrics
var metricsBuilder = new MetricsBuilder()
        .OutputMetrics.AsPrometheusProtobuf();

var metrics = metricsBuilder.Build();

//Enable web stats metrics
builder.WebHost.UseMetricsWebTracking();

//Output to /metrics
builder.WebHost.UseMetrics(options => { options.EndpointOptions = endpointOptions =>
{
    endpointOptions.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
    endpointOptions.EnvironmentInfoEndpointEnabled = false;
}; });

builder.Services.AddMetrics(metrics);






// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




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




