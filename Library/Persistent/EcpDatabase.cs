using System.Reflection;
using Library.Persistent.Entities;
using Library.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Library.Persistent;

public class EcpDatabase(DbContextOptions<EcpDatabase> options)
    : IdentityDbContext<UserEntity, RoleEntity, string>(options), IEcpDatabase
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<IdentityUserLogin<string>>().HasKey(x => new { x.LoginProvider, x.ProviderKey });
        builder.Entity<IdentityUserRole<string>>().HasKey(x => new { x.UserId, x.RoleId });
        builder.Entity<IdentityUserToken<string>>().HasKey(x => new { x.UserId, x.LoginProvider, x.Name });
        
        builder.Entity<UserEntity>().ToTable("Users");
        builder.Entity<RoleEntity>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokes");
        
        builder.Entity<UserEntity>()
            .Property(u => u.FullName)
            .HasMaxLength(100)
            .IsRequired();
        
        var cascade = builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys())
            .Where(e => e.DeleteBehavior == DeleteBehavior.Cascade);
        foreach (var c in cascade) c.DeleteBehavior = DeleteBehavior.NoAction;
        
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
    
    
    Task<int> IEcpDatabase.SaveChanges()
    {
        return Task.FromResult(SaveChanges());
    }
}