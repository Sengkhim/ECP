namespace API_GateWay.core.extensions;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Logs;

public static class TelemetryService
{
    public static void AddTelemetryServices(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceName = configuration["OpenTelemetry:ServiceName"] ?? "API-GATEWAY";
        var jaegerHost = configuration["OpenTelemetry:JaegerHost"] ?? "localhost";
        var jaegerPort = int.Parse(configuration["OpenTelemetry:JaegerPort"] ?? "44317");

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource
                .AddService(serviceName)
                .AddTelemetrySdk()
            )
            .WithTracing(tracing => tracing
                    .AddAspNetCoreInstrumentation()    // Trace API requests
                    .AddHttpClientInstrumentation()   // Trace outgoing HTTP calls
                    .AddSqlClientInstrumentation()   // Trace DB queries
                    .AddSource(serviceName)          // Custom tracing
                    .SetSampler(new AlwaysOnSampler()) // Capture all traces
                    .AddJaegerExporter(o => {
                        o.AgentHost = jaegerHost;
                        o.AgentPort = jaegerPort;
                    })
                    .AddConsoleExporter()  // Optional for debugging
            )
            .WithMetrics(metrics => metrics
                    .AddAspNetCoreInstrumentation() // Collect HTTP metrics
                    .AddHttpClientInstrumentation() // Collect HTTP client metrics
                    .AddMeter(serviceName)          // Custom application metrics
                    .AddPrometheusExporter()        // Expose metrics for Prometheus
            );

        // Enable logging with OpenTelemetry
        services.Configure<OpenTelemetryLoggerOptions>(logging => {
            logging.IncludeScopes = true;
            logging.ParseStateValues = true;
            logging.AddConsoleExporter();
        });
    }
}
