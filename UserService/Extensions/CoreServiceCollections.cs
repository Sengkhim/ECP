using Library.Core.Configuration;
using Library.Core.UnitOfWork;
using Library.Extensions;
using Library.Persistent.Entities;
using Library.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Database;

namespace UserService.Extensions;

public static class CoreServiceCollections
{
    public static void AddCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConfigurationModeling, ConfigurationModeling>();
        // Register UnitOfWork with DI, passing UserDbContext instance
        // services.AddScoped<IUnitOfWork<UserDbContext>, UnitOfWork<UserDbContext>>();


        services.AddGlobalCoreServices();
        services.AddSwaggerUi();
        
        // services.AddDbContext<IEcpDatabase, UserDbContext>(options =>
        //     options.UseNpgsql(configuration.GetConnectionString("ECP_USER_DB")));
        //
        // services.AddIdentity<UserEntity, RoleEntity>()
        //     .AddEntityFrameworkStores<UserDbContext>()
        //     .AddDefaultTokenProviders();
        
        // services.AddScoped<IEcpDatabase, UserDbContext>();
    }
}