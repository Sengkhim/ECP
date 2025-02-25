using System.Linq.Expressions;

namespace Library.Core.Repository;

public interface IRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> Entities { get; }
    
    Task<TEntity?> GetByIdAsync(string id);
    
    Task<IEnumerable<TEntity>> GetAllAsync();
    
    Task<IQueryable<TEntity>> GetPagedResponseAsync(int pageNumber, int pageSize);
    
    Task<TEntity> AddAsync(TEntity entity);
    
    Task UpdateAsync(TEntity entity);
    
    Task DeleteAsync(TEntity entity);
    
    Task DeleteByIdAsync(string id);
    
    Task<IQueryable<TEntity>?> FindAsync(Expression<Func<TEntity, bool>> expression);
}