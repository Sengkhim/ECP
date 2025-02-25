using Microsoft.AspNetCore.Identity;

namespace Library.Persistent.Entities;

public class RoleEntity : IdentityRole
{
    public string Description { get; set; } = string.Empty;
}