using Library.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Extensions;

public static class ApplicationExtension
{
    public static void ApplyMigrate(this IApplicationBuilder app, bool development = true)
    {
        if (!development) return;
        
        using var scope = app.ApplicationServices.CreateAsyncScope();
        var service = scope
            .ServiceProvider
            .GetRequiredService<IEcpDatabase>();
        service.Database.Migrate();
    }
}