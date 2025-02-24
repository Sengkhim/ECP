using Consul;
using UserService.Services;

var builder = WebApplication.CreateBuilder(args);

var serviceId = $"UserService-{Guid.NewGuid()}";

// Register Consul Client
builder.Services.AddSingleton<IConsulClient, ConsulClient>(p => new ConsulClient(config =>
{
    config.Address = new Uri("http://localhost:8500"); // Consul URL
}));

builder.Services.AddSingleton<ConsulServiceDiscovery>();
// Register HttpClient for service-to-service calls
builder.Services.AddHttpClient("ServiceClient");

// Add Controllers
builder.Services.AddControllers();

var app = builder.Build();

// Register service in Consul on startup
using (var scope = app.Services.CreateScope())
{
    var consulClient = scope.ServiceProvider.GetRequiredService<IConsulClient>();

    var registration = new AgentServiceRegistration
    {
        ID = serviceId,
        Name = "UserService",
        Address = "localhost",
        Port = 5268, // Ensure this matches your running service port
        Check = new AgentServiceCheck
        {
            HTTP = "http://localhost:5268/health",
            Interval = TimeSpan.FromSeconds(10),
            Timeout = TimeSpan.FromSeconds(5)
        }
    };

    consulClient.Agent.ServiceRegister(registration).Wait();
}

app.MapControllers();

// Health Check Endpoint
app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();

// Deregister service on shutdown
app.Lifetime.ApplicationStopping.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var consulClient = scope.ServiceProvider.GetRequiredService<IConsulClient>();
    consulClient.Agent.ServiceDeregister(serviceId).Wait();
});