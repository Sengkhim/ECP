namespace API_GateWay.core.Configuration.Oauth2;

public class GitHubConfiguration
{
    public string ClientId { get; set; } = string.Empty;
    
    public string ClientSecret { get; set; } = string.Empty;
    public string AuthorizationEndpoint { get; set; } = string.Empty;
    public string TokenEndpoint  { get; set; } = string.Empty;
    public string UserInformationEndpoint   { get; set; } = string.Empty;
}