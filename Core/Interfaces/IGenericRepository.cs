using Core.Entities;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;


namespace Core.Interfaces;
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(int id, List<Expression<Func<T, object>>>? includes=null);
    Task<IReadOnlyList<T>> ListAllAsync(List<Expression<Func<T, object>>>? includes = null);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T> GetByConditionAsync(Expression<Func<T, bool>> expression, List<Expression<Func<T, object>>>? includes=null);
    Task<IReadOnlyList<T>> ListAllByConditionAsync(Expression<Func<T, bool>> expression, List<Expression<Func<T, object>>>? includes = null);
}