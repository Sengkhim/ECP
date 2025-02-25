using System.Collections;
using Library.Core.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static System.GC;

namespace Library.Core.UnitOfWork;


public class UnitOfWork(DbContext context) : IUnitOfWork
{       
    private Hashtable? _repositories;
    private bool _disposed;

    public DbContext Context => context;

    public async Task<int> CommitAsync(CancellationToken cancellationToken) 
        => await context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        Dispose(true);
        SuppressFinalize(this);
    }
    
    private void Dispose(bool disposing)
    {
        if (!_disposed)
            if (disposing) context.Dispose();
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
        Context.ChangeTracker
            .Entries()
            .ToList()
            .ForEach(entry => entry.Reload());
        return Task.CompletedTask;
    }
}