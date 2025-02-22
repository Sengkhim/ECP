using API_GateWay.core.Middleware;
using API_GateWay.Infrastructure.Extension;

namespace API_GateWay.core.extensions;

public static class ApplicationExtension
{
    private static void UseDistributedCache(this WebApplication app)
    {
        app.Use(DistributedCacheMiddleware.UseCache);
    }

    public static void AddApplicationMiddlewares(this WebApplication app)
    {
        app.ApplyMigrate();
        app.UseServiceRegistries();
        app.UseResponseCaching();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseRateLimiter();
        app.MapReverseProxy();
        app.UseDistributedCache();
        app.MapHealthChecks("/health");
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
    }
}