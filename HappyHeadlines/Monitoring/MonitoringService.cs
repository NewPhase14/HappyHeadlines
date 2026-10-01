using System.Diagnostics;
using System.Reflection;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace Monitoring;

public static class MonitoringService
{
    public static readonly string ServiceName = Assembly.GetCallingAssembly().GetName().Name ?? "Unknown";
    public static TracerProvider TracerProvider;
    public static ActivitySource ActivitySource = new(ServiceName);

    static MonitoringService()
    {
        //OpenTelemetry
        TracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(ServiceName))
            .AddSource(ServiceName)
            // Force all activities to be sampled
            .SetSampler(new AlwaysOnSampler())
            .AddConsoleExporter()
            .AddZipkinExporter(config => { config.Endpoint = new Uri("http://zipkin:9411/api/v2/spans"); })
            .Build();

        //Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.Console()
            .WriteTo.Seq("http://seq:5341")
            .CreateLogger();
    }

    public static ILogger log => Log.Logger;
}