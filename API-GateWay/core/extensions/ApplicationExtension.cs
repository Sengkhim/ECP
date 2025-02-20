using API_GateWay.core.Services;

namespace API_GateWay.core.extensions;

public static class ApplicationExtension
{
    private static void UseDistributedCache(this WebApplication app)
    {
        app.Use(async (context, next) =>
        {
            var cacheService = context.RequestServices.GetRequiredService<IRedisCacheService>();
            var cacheKey = context.Request.Path.ToString();

            // Try getting cached response
            var cachedResponse = await cacheService.GetCachedResponseAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedResponse))
            {
                // Set content type for cached response
                context.Response.ContentType = "application/json"; 
                await context.Response.WriteAsync(cachedResponse);
                return;
            }

            // Capture the response stream
            var originalBodyStream = context.Response.Body;
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            await next(); // Process request and get response

            // Read response from memory stream
            memoryStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

            // Cache the response
            await cacheService.SetCachedResponseAsync(cacheKey, responseBody, TimeSpan.FromSeconds(60));

            // Reset the response stream
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalBodyStream);
        });
    }

    public static void AddApplicationMiddlewares(this WebApplication app)
    {
        app.UseServiceRegistries();
        app.UseResponseCaching();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimiter();
        app.MapReverseProxy();
        app.UseDistributedCache();
        app.MapHealthChecks("/health");
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
        app.MapControllers();
    }
}