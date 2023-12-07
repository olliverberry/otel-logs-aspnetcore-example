using OpenTelemetry.Logs;
using OpenTelemetry.Resources;

namespace DotNetApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders();
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeScopes = true;

            var resourceBuilder = ResourceBuilder
                .CreateDefault()
                .AddService(builder.Environment.ApplicationName);

            logging.SetResourceBuilder(resourceBuilder)
                .AddOtlpExporter(exporter => 
                {
                    var nodeName = Environment.GetEnvironmentVariable("NODE_NAME") ?? "localhost";
                    exporter.Endpoint = new Uri($"http://{nodeName}:4317");
                });
        });
        builder.Services.AddControllers();

        var app = builder.Build();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
