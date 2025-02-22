using API_GateWay.Infrastructure.Database;
using API_GateWay.Infrastructure.Entities.Identities;
using API_GateWay.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API_GateWay.Infrastructure.Extension;

public static class DatabaseExtension
{
    private static void AddServiceCollections(this IServiceCollection service)
    {
        service.AddScoped<IEcpDatabase, EcpDatabase>();
    }
    
    public static void AddEpcDatabase(this IServiceCollection service, IConfiguration config)
    {
        service.AddServiceCollections();
        
        var connectionString = config.GetConnectionString("ECP_DATABASE");
        service.AddDbContext<IEcpDatabase, EcpDatabase>(options => options.UseNpgsql(connectionString));
        
        service.AddIdentity<UserEntity, RoleEntity>()
            .AddEntityFrameworkStores<EcpDatabase>()
            .AddDefaultTokenProviders();
    }
}