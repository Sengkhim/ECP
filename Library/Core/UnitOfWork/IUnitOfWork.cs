using Library.Core.Repository;
using Library.Service;

namespace Library.Core.UnitOfWork;

public interface IUnitOfWork : IDisposable 
{
    IEcpDatabase Context { get; }
    
    IRepository<T> Repository<T>() where T : class;
    
    Task<int> CommitAsync(CancellationToken cancellationToken);
    
    Task Rollback();
}