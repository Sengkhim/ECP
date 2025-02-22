namespace API_GateWay.core.Configuration.Oauth2;

public class GoogleConfiguration : GitHubConfiguration
{
    public string CallbackPath { get; set; } = string.Empty;
}