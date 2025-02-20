using System.Security.Claims;

namespace API_GateWay.core.DTOs;

public class GitHubUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string ProfileUrl { get; set; } = string.Empty;
    public string AvatarUrl { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;

    public static GitHubUserDto GitHubResponse(IEnumerable<Claim>? claims)
    {
        var enumerable = claims as Claim[] ?? claims?.ToArray();
        return new GitHubUserDto
        {
            Id = enumerable?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "",
            Name = enumerable?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? "",
            Username = enumerable?.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value ?? "",
            Email = enumerable?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? "",
            ProfileUrl = enumerable?.FirstOrDefault(c => c.Type == ClaimTypes.Webpage)?.Value ?? "",
            AvatarUrl = enumerable?.FirstOrDefault(c => c.Type == "avatar_url")?.Value ?? "",
            Bio = enumerable?.FirstOrDefault(c => c.Type == "bio")?.Value ?? ""
        };
    }
}
