using Consul;
namespace UserService.Services;

public class ConsulServiceDiscovery(IConsulClient consulClient)
{
    public async Task<string?> GetServiceUriAsync(string serviceName)
    {
        var services = await consulClient.Agent.Services();
        var service = services.Response
            .Values
            .FirstOrDefault(s => s.Service.Equals(serviceName, StringComparison.OrdinalIgnoreCase));

        return service != null ? $"http://{service.Address}:{service.Port}" : null;
    }
}
