using System.Collections;
using Library.Core.Repository;
using Library.Persistent;
using Library.Service;

namespace Library.Core.UnitOfWork;

public class UnitOfWork(EcpDatabase context) : IUnitOfWork
{       
    private Hashtable? _repositories;
    private bool _disposed;

    public IEcpDatabase Context { get; } = context;

    public async Task<int> CommitAsync(CancellationToken cancellationToken) 
        => await Context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    private void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing)
                //dispose managed resources
                context.Dispose();
        //dispose unmanaged resources
        _disposed = true;
    }

    public IRepository<T> Repository<T>() where T : class
    {
        _repositories ??= new Hashtable();

        var type = typeof(T).Name;

        if (_repositories.ContainsKey(type)) return (IRepository<T>)_repositories[type]!;
            
        var repositoryType = typeof(Repository<T>);

        var repositoryInstance = Activator.CreateInstance(repositoryType, Context);

        _repositories.Add(type, repositoryInstance);

        return (IRepository<T>)_repositories[type]!;
    }

    public Task Rollback()
    {
        context.ChangeTracker
            .Entries()
            .ToList()
            .ForEach(entry => entry.Reload());
        return Task.CompletedTask;
    }
}