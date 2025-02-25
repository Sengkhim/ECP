using Library.Core.Configuration;
using Library.Core.UnitOfWork;
using Library.Persistent;
using Library.Persistent.Entities;
using Library.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddGlobalCoreServices(this IServiceCollection services)
    {
        services.AddScoped<IEcpDatabase, EcpDatabase>();
    }

    public static void LoadCoreServices(this IServiceCollection services)
    {
        services.AddSingleton<IConfigurationModeling, ConfigurationModeling>();
    }
    
    public static void AddDatabaseLayer(this IServiceCollection services, IConfiguration config, string key = "ECP_DATABASE")
    {
        var connectionString = config.GetConnectionString(key);

        services.AddDbContext<EcpDatabase>(options =>
            options.UseNpgsql(connectionString));

        services.AddIdentity<UserEntity, RoleEntity>()
            .AddEntityFrameworkStores<EcpDatabase>()
            .AddDefaultTokenProviders();
    }
}