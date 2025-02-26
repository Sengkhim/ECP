using System.Net;
using API_GateWay.core.Configuration.common;
using Consul;

namespace API_GateWay.core.extensions;

public static class ConsulServiceExtension
{
    public static void AddConsulClient(this IServiceCollection service, IConfiguration configuration)
    {
        var consulConfig = configuration.GetSection("ConsulClient").Get<ConsulConfiguration>();
        service.AddSingleton<IConsulClient, ConsulClient>(p => 
            new ConsulClient(cfg => 
                cfg.Address = new Uri(consulConfig?.Address ?? "http://localhost:8500"))); 
    }

    public static void UseServiceRegistries(this WebApplication app)
    {
        // Register Service on Start
        app.Lifetime.ApplicationStarted.Register(() =>
        {
            using var scope = app.Services.CreateScope();
            var consulClient = scope.ServiceProvider.GetRequiredService<IConsulClient>();
            var registration = new AgentServiceRegistration
            {
                ID = $"service-{Dns.GetHostName()}",
                Name = "API-GATEWAY-SERVICE",
                Address = "localhost",
                Port = 7277, // Change if needed
                Check = new AgentServiceCheck
                {
                    HTTP = "https://localhost:7277/health",
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5)
                }
            };

            consulClient.Agent.ServiceRegister(registration).Wait();
        });
        
        // Unregister Service on Stop
        app.Lifetime.ApplicationStopping.Register(() =>
        {
            using var scope = app.Services.CreateScope();
            var consulClient = scope.ServiceProvider.GetRequiredService<IConsulClient>();
            consulClient.Agent.ServiceDeregister($"service-{Dns.GetHostName()}").Wait();
        });
    }
}