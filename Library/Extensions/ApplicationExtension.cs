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

    public static void ApplyMigrate(this bool development, IApplicationBuilder app)
    {
        if (!development) return;
        
        using var scope = app.ApplicationServices.CreateAsyncScope();
        
        var service = scope
            .ServiceProvider
            .GetRequiredService<IEcpDatabase>();
        
        try
        {
            if (service.Database.GetPendingMigrations().Any())
            {
                service.Database.Migrate();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
        }
    }
}