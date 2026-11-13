using System.Diagnostics;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Monitoring;

public static class MonitoringService
{
    public static TracerProvider TracerProvider = null!;
    public static ActivitySource ActivitySource = null!;

    public static void Configure(string serviceName)
    {
        ActivitySource = new ActivitySource(serviceName);

        TracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName))
            .AddSource(serviceName)
            .SetSampler(new AlwaysOnSampler())
            .AddConsoleExporter()
            .AddZipkinExporter(config =>
            {
                config.Endpoint =
                    new Uri("http://zipkin:9411/api/v2/spans");
            })
            .Build();

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.Seq("http://seq:5341")
            .CreateLogger();
    }

    public static ILogger log => Log.Logger;
}







