using API_GateWay.core.extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTelemetryServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFixedWindowRateLimiter();
builder.Services.AddResponseCaching();
builder.Services.AddHealthChecks()
    .AddCheck("API Gateway Health", () => HealthCheckResult.Healthy());
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.AddLogging();
builder.Services.AddConsulClient();

var app = builder.Build();

app.UseServiceRegistries();
app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapReverseProxy();
app.MapHealthChecks("/health");
app.UseOpenTelemetryPrometheusScrapingEndpoint();
app.Run();