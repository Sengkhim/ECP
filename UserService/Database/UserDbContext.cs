using Library.Service;
using Microsoft.EntityFrameworkCore;

namespace UserService.Database;

public class UserDbContext : DbContext
{
    private readonly IConfigurationModeling _modeling;

    public UserDbContext(DbContextOptions<UserDbContext> options, IConfigurationModeling modeling)
        : base(options)
    {
        _modeling = modeling;
    }

    // Add DbSets and other configurations here
}

// public class UserDbContext(DbContextOptions<UserDbContext> options, IConfigurationModeling modeling)
//     : DbContext(options), IEcpDatabase
// {
//     protected override void OnModelCreating(ModelBuilder model)
//     {
//         base.OnModelCreating(model);
//         modeling.Configuration(model);
//     }
//
//     Task<int> IEcpDatabase.SaveChanges()
//     {
//         return Task.FromResult(SaveChanges());
//     }
// }
