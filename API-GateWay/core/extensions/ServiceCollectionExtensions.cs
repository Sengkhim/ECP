using API_GateWay.core.Configuration.Caches;
using API_GateWay.core.Configuration.common;
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
    private static void AddFixedWindowRateLimiter(
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
    /// <summary>
    /// Represents a configuration services with oauth 2
    /// </summary>
    /// <param name="service">The IServiceCollection instance.</param>
    /// <param name="configuration"></param> Represents a set of key/ value application configuration properties.
    private static void AddOAuthServices(this IServiceCollection service, IConfiguration configuration)
    {
        // service.AddFacebookOAuth2(configuration);
        // service.AddGoogleOAuth2(configuration);
        service.AddGitHubOAuth2(configuration);    
    }
    /// <summary>
    /// Represents a configuration application settings
    /// </summary>
    /// <param name="service">The IServiceCollection instance.</param>
    /// <param name="configuration"></param> Represents a set of key/ value application configuration properties.
    private static void AddConfigurations(this IServiceCollection service, IConfiguration configuration)
    {
        service.Configure<GitHubConfiguration>(configuration.GetSection("GitHub"));
        service.Configure<GoogleConfiguration>(configuration.GetSection("Google"));
        service.Configure<ConsulConfiguration>(configuration.GetSection("ConsulClient"));
        service.Configure<RedisConfiguration>(configuration.GetSection("Redis"));  
    }

    public static void AddServiceCollections(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddMemoryCache();
        service.AddConfigurations(configuration);
        
        service.AddHealthChecks()
            .AddCheck("API Gateway Health", () => HealthCheckResult.Healthy());

        service
            .AddReverseProxy()
            .LoadFromConfig(configuration.GetSection("ReverseProxy"));
        
        var redisConfig = configuration.GetSection("Redis").Get<RedisConfiguration>();
        
        service.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConfig?.Address ?? "localhost:6379";
            options.InstanceName = redisConfig?.InstanceName ?? "G_Cache";
        });

        service.AddSingleton<IRedisCacheService, RedisCacheService>();

        service.AddFixedWindowRateLimiter();
        service.AddTelemetryServices(configuration);
        service.AddConsulClient(configuration);
        service.AddOAuthServices(configuration);
    }
}