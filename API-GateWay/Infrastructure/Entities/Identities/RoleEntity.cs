using Microsoft.AspNetCore.Identity;

namespace API_GateWay.Infrastructure.Entities.Identities;

public class RoleEntity : IdentityRole
{
    public string Description { get; set; } = string.Empty;
}