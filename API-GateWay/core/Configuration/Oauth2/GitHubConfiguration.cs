namespace API_GateWay.core.Configuration.Oauth2;

public class GitHubConfiguration
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string AuthorizationEndpoint { get; init; } = string.Empty;
    public string TokenEndpoint  { get; init; } = string.Empty;
    public string UserInformationEndpoint  { get; init; } = string.Empty;
}