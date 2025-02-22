using Microsoft.AspNetCore.Identity;

namespace API_GateWay.Infrastructure.Entities.Identities;

public class UserEntity : IdentityUser
{
    public string FullName { get; init; } = string.Empty;
    public DateTime DateOfBirth { get; init; }
    public string Address { get; init; } = string.Empty;
}