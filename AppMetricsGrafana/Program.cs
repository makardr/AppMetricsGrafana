using System.Net.Http.Headers;
using System.Text;
using App.Metrics;
using AppMetricsGrafana.Metrics;


var apiCode = "";

var builder = WebApplication.CreateBuilder(args);

var metricsBuilder = new MetricsBuilder()
        .Report.ToConsole()
        .OutputMetrics.AsPlainText()
        .OutputMetrics.AsJson()
        .OutputMetrics.AsPrometheusPlainText()
        .OutputMetrics.AsPrometheusProtobuf()
    ;

var metrics = metricsBuilder.Build();

builder.Services.AddMetrics(metrics);

builder.WebHost.UseMetricsWebTracking();

//Output to /metrics
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


var snapshot = metrics.Snapshot.Get();


foreach (var formatter in metrics.OutputMetricsFormatters)
{
    using (var stream = new MemoryStream())
    {
        await formatter.WriteAsync(stream, snapshot);
        var httpContent = new ByteArrayContent(stream.ToArray());
        //Output result as text in the console
        var result = Encoding.UTF8.GetString(stream.ToArray());
        Console.WriteLine(result);

        SendRequest(httpContent);
    }
}


app.Run();


metrics.Measure.Counter.Increment(MetricsRegistry.ExampleCounterOptions, 1);

async void SendRequest(HttpContent httpContent)
{
    HttpClient client = new HttpClient();
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiCode);
    HttpResponseMessage response =
        await client.PostAsync("https://metrics.cockpit.fr-par.scw.cloud/api/v1/push", httpContent);
    Console.WriteLine(response.StatusCode);
    string responseContent = await response.Content.ReadAsStringAsync();
    Console.WriteLine(responseContent);
}