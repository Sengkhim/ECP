using API_GateWay.core.extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddFixedWindowRateLimiter();
builder.Services.AddResponseCaching();
builder.AddLogging();
builder.Services.AddHealthChecks()
    .AddCheck("API Gateway Health", () => HealthCheckResult.Healthy());

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.MapReverseProxy();
app.MapHealthChecks("/health");
app.Run();