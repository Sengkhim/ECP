using Library.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace Library.Core.UnitOfWork;

public interface IUnitOfWork : IDisposable 
{
    DbContext Context { get; }
    
    IRepository<T> Repository<T>() where T : class;
    
    Task<int> CommitAsync(CancellationToken cancellationToken);
    
    Task Rollback();
}