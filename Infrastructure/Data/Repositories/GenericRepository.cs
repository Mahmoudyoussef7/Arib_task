using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;

namespace Infrastructure.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly AppDbContext _context;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<T> GetByIdAsync(int id, List<Expression<Func<T, object>>>? includes = null)
    {
        var query = _context.Set<T>().AsQueryable();

        if (includes != null && includes.Any())
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        return await query.FirstOrDefaultAsync(i => EF.Property<int>(i, "Id") == id)
                ?? throw new Exception("Not Found");
    }

    public async Task<IReadOnlyList<T>> ListAllAsync(List<Expression<Func<T, object>>>? includes = null)
    {
        var query = _context.Set<T>().AsQueryable();
        if (includes != null && includes.Any())
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }
        return await query.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<T> GetByConditionAsync(Expression<Func<T, bool>> expression, List<Expression<Func<T, object>>>? includes = null)
    {
        var query = _context.Set<T>().AsQueryable();

        if (includes != null && includes.Any())
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }

        return await query.FirstOrDefaultAsync(expression)?? throw new Exception("Not Found");
    }

    public async Task<IReadOnlyList<T>> ListAllByConditionAsync(Expression<Func<T, bool>> expression, List<Expression<Func<T, object>>>? includes = null)
    {
        var query = _context.Set<T>().Where(expression);
        if(includes != null && includes.Any())
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include));
        }
        return await query.ToListAsync();
    }
}
