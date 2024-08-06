using System.Linq.Expressions;
using Core.Specifications;

namespace Core.Interfaces
{
    public interface IBaseRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(bool withNoTracking = true);
    IQueryable<T> GetQueryable();
    //PaginatedList<T> GetPaginatedList(IQueryable<T> query, int pageNumber, int pageSize);
    Task<T>? GetByIdAsync(int id);
    T? Find(Expression<Func<T, bool>> predicate);
    T? Find(Expression<Func<T, bool>> predicate, string[]? includes = null);
    Task<T> GetEntityWithSpec(ISpecification<T> spec);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
    Task<int> GetCountAsync(ISpecification<T> spec);
    // T? Find(Expression<Func<T, bool>> predicate,
    //         Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
    IEnumerable<T> FindAll(Expression<Func<T, bool>> predicate, int? skip = null, int? take = null,
        Expression<Func<T, object>>? orderBy = null, string? orderByDirection = "ASC");
    IEnumerable<T> FindAll(Expression<Func<T, bool>> predicate,
        Expression<Func<T, object>>? orderBy = null, string? orderByDirection = "ASC");
    // IEnumerable<T> FindAll(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
    //     Expression<Func<T, object>>? orderBy = null, string? orderByDirection = OrderBy.Ascending);
    T Add(T entity);
    IEnumerable<T> AddRange(IEnumerable<T> entities);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
    void DeleteBulk(Expression<Func<T, bool>> predicate);
    bool IsExists(Expression<Func<T, bool>> predicate);
    int Count();
    int Count(Expression<Func<T, bool>> predicate);
    int Max(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> field);
}
}