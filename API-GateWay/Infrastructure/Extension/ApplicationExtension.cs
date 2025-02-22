using API_GateWay.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace API_GateWay.Infrastructure.Extension;

public static class ApplicationExtension
{
    public static void ApplyMigrate(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return;
        using var scope = app.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<EcpDatabase>();
        service.Database.Migrate();
    }
}