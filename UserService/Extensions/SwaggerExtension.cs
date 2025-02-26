using Microsoft.OpenApi.Models;

namespace UserService.Extensions;

public static class SwaggerExtension
{
    public static void AddSwaggerUi(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "USER SERVICE SCHEMA",
                Version = "v1",
                Description = "USER SERVICE SCHEMA"
            });
        });
    }
}