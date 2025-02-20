using API_GateWay.core.Configuration.Oauth2;
using API_GateWay.core.implement;
using API_GateWay.core.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
namespace API_GateWay.core.extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures Serilog as the logging provider for the application.
    /// This method initializes Serilog with console logging and sets it as the default logger.
    /// </summary>
    /// <param name="builder">The WebApplicationBuilder instance.</param>
    /// <returns>The modified WebApplicationBuilder instance to allow method chaining.</returns>
    public static void AddLogging(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();
        builder.Host.UseSerilog();
    }
    /// <summary>
    /// Configures a fixed window rate limiter for the application.
    /// This rate limiter allows a specified number of requests within a given time window.
    /// </summary>
    /// <param name="services">The IServiceCollection instance.</param>
    /// <param name="policyName">The name of the rate limiter policy.</param>
    /// <param name="windowSeconds">The duration of the time window in seconds.</param>
    /// <param name="permitLimit">The maximum number of requests allowed in the time window.</param>
    /// <returns>The IServiceCollection instance to allow method chaining.</returns>
    public static void AddFixedWindowRateLimiter(
        this IServiceCollection services, 
        string policyName = "fixed", 
        int windowSeconds = 10, 
        int permitLimit = 5)
    {
        services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter(policyName, limiterOptions =>
            {
                limiterOptions.Window = TimeSpan.FromSeconds(windowSeconds);
                limiterOptions.PermitLimit = permitLimit;
            });
        });
    }


    public static void AddServiceCollections(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddHealthChecks()
            .AddCheck("API Gateway Health", () => HealthCheckResult.Healthy());

        service
            .AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));
        
        service.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "localhost:6379";
            options.InstanceName = "YardCache";
        });

        service.AddSingleton<IRedisCacheService, RedisCacheService>();
        
        service.Configure<GitHubConfiguration>(configuration.GetSection("GitHub"));
        service.AddGitHubOAuth2(configuration);
    }
}