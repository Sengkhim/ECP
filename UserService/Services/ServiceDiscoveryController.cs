using Microsoft.AspNetCore.Mvc;
namespace UserService.Services;

[ApiController]
[Route("api/discovery")]
public class ServiceDiscoveryController(
    ConsulServiceDiscovery consulServiceDiscovery,
    IHttpClientFactory httpClientFactory)
    : ControllerBase
{
    [HttpGet("{serviceName}")]
    public async Task<IActionResult> GetServiceUrl(string serviceName)
    {
        var serviceUri = await consulServiceDiscovery.GetServiceUriAsync(serviceName);
        return serviceUri != null ? Ok(serviceUri) : NotFound($"Service '{serviceName}' not found in Consul.");
    }

    [HttpGet("call/{serviceName}")]
    public async Task<IActionResult> CallService(string serviceName)
    {
        var serviceUri = await consulServiceDiscovery.GetServiceUriAsync(serviceName);
        if (serviceUri == null) return NotFound($"Service '{serviceName}' not found.");

        var client = httpClientFactory.CreateClient();
        var response = await client.GetStringAsync($"{serviceUri}/api/users");

        return Ok(response);
    }
}
