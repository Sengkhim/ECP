namespace API_GateWay.core.Configuration.Caches;

public class RedisConfiguration
{
    public string Address { get; init ; } = string.Empty;
    
    public string Port { get; init; } = string.Empty;  
    public string InstanceName { get; init; } = string.Empty;
}