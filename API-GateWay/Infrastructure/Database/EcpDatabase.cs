using System.Reflection;
using API_GateWay.Infrastructure.Entities.Identities;
using API_GateWay.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API_GateWay.Infrastructure.Database;

public class EcpDatabase(DbContextOptions<EcpDatabase> options)
    : IdentityDbContext<UserEntity, RoleEntity, string>(options), IEcpDatabase
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<IdentityUserLogin<string>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });
        builder.Entity<IdentityUserRole<string>>().HasKey(x => new { x.UserId, x.RoleId });
        builder.Entity<IdentityUserToken<string>>().HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
        
        builder.Entity<UserEntity>().ToTable("users");
        builder.Entity<RoleEntity>().ToTable("roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("user_roles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("user_claims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("user_Logins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claims");
        builder.Entity<IdentityUserToken<string>>().ToTable("user_tokens");
        
        builder.Entity<UserEntity>()
            .Property(u => u.FullName)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        var cascade = builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())
            .Where(e => e.DeleteBehavior == DeleteBehavior.Cascade);
        foreach (var c in cascade) c.DeleteBehavior = DeleteBehavior.NoAction;
    }

    Task<int> IEcpDatabase.SaveChanges()
    {
        return Task.FromResult(SaveChanges());
    }
}