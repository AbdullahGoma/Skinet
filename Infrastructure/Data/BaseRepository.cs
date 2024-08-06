using System.Linq.Expressions;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class BaseRepository<T>: IBaseRepository<T> where T : class
{
	protected readonly StoreContext _context;

	public BaseRepository(StoreContext context)
	{
		_context = context;
	}

	public async Task<IEnumerable<T>> GetAllAsync(bool withNoTracking = true)
	{
		IQueryable<T> query = _context.Set<T>();

		if (withNoTracking)
			query = query.AsNoTracking();

		return await query.ToListAsync();
	}

	public IQueryable<T> GetQueryable()
	{
		return _context.Set<T>();
	}

	//public PaginatedList<T> GetPaginatedList(IQueryable<T> query, int pageNumber, int pageSize)
	//{
	//	return PaginatedList<T>.Create(query, pageNumber, pageSize);
	//}

	public async Task<T>? GetByIdAsync(int id) => await _context.Set<T>().FindAsync(id); 

	public T? Find(Expression<Func<T, bool>> predicate) =>
		_context.Set<T>().SingleOrDefault(predicate);

	public T? Find(Expression<Func<T, bool>> predicate, string[]? includes = null)
	{
		IQueryable<T> query = _context.Set<T>();

		if (includes is not null)
			foreach (var include in includes)
				query = query.Include(include);

		return query.SingleOrDefault(predicate);
	}

	public async Task<T> GetEntityWithSpec(ISpecification<T> spec)
	{
		return await ApplySpecification(spec).FirstOrDefaultAsync();
	}

	public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
	{
		return await ApplySpecification(spec).ToListAsync();
	}
	public async Task<int> GetCountAsync(ISpecification<T> spec)
	{
		return await ApplySpecification(spec).CountAsync();
	}

	// public T? Find(Expression<Func<T, bool>> predicate,
	// 		Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
	// {
	// 	IQueryable<T> query = _context.Set<T>().AsQueryable();

	// 	if (include is not null)
	// 		query = include(query);

	// 	return query.SingleOrDefault(predicate);
	// }

	public IEnumerable<T> FindAll(Expression<Func<T, bool>> predicate,
		Expression<Func<T, object>>? orderBy = null, string? orderByDirection = "ASC")
	{
		IQueryable<T> query = _context.Set<T>().Where(predicate);

		if (orderBy is not null)
		{
			if (orderByDirection == "ASC")
				query = query.OrderBy(orderBy);
			else
				query = query.OrderByDescending(orderBy);
		}

		return query.ToList();
	}

	public IEnumerable<T> FindAll(Expression<Func<T, bool>> predicate, int? skip = null, int? take = null,
		Expression<Func<T, object>>? orderBy = null, string? orderByDirection = "ASC")
	{
		IQueryable<T> query = _context.Set<T>().Where(predicate);

		if (orderBy is not null)
		{
			if (orderByDirection == "ASC")
				query = query.OrderBy(orderBy);
			else
				query = query.OrderByDescending(orderBy);
		}

		if (skip.HasValue)
			query = query.Skip(skip.Value);

		if (take.HasValue)
			query = query.Take(take.Value);

		return query.ToList();
	}

	// public IEnumerable<T> FindAll(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
	// 	Expression<Func<T, object>>? orderBy = null, string? orderByDirection = OrderBy.Ascending)
	// {
	// 	IQueryable<T> query = _context.Set<T>().AsQueryable();

	// 	if (include is not null)
	// 		query = include(query);

	// 	query = query.Where(predicate);

	// 	if (orderBy is not null)
	// 	{
	// 		if (orderByDirection == OrderBy.Ascending)
	// 			query = query.OrderBy(orderBy);
	// 		else
	// 			query = query.OrderByDescending(orderBy);
	// 	}

	// 	return query.ToList();
	// }

	public T Add(T entity)
	{
		_context.Add(entity);
		return entity;
	}

	public IEnumerable<T> AddRange(IEnumerable<T> entities)
	{
		_context.AddRange(entities);
		return entities;
	}

	public void Update(T entity) => _context.Update(entity);

	//.NET 6
	public void Remove(T entity) => _context.Remove(entity);

	//.NET 6
	public void RemoveRange(IEnumerable<T> entities) => _context.RemoveRange(entities);

	//.NET 7
	public void DeleteBulk(Expression<Func<T, bool>> predicate) =>
		_context.Set<T>().Where(predicate).ExecuteDelete();

	public bool IsExists(Expression<Func<T, bool>> predicate) =>
		_context.Set<T>().Any(predicate);

	public int Count() => _context.Set<T>().Count();

	public int Count(Expression<Func<T, bool>> predicate) => _context.Set<T>().Count(predicate);

	public int Max(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> field) =>
		_context.Set<T>().Any(predicate) ? _context.Set<T>().Where(predicate).Max(field) : 0;

	private IQueryable<T> ApplySpecification(ISpecification<T> spec) 
	{
		return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
	}

    }
}