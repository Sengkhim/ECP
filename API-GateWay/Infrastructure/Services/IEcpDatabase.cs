using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace API_GateWay.Infrastructure.Services;

public interface IEcpDatabase
{
    DatabaseFacade Database { get; }
    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
    EntityEntry<TEntity> Remove<TEntity>(TEntity entity) where TEntity : class;
    EntityEntry<TEntity> Attach<TEntity>(TEntity entity) where TEntity : class;
    /// <summary>
    ///     Saves the changes made to the context.
    /// </summary>
    /// <returns></returns>
    Task<int> SaveChanges();

    /// <summary>
    ///     Saves the changes made to the context.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}