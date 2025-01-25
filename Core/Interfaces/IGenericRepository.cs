using Core.Entities;
using System.Linq.Expressions;
using Task = System.Threading.Tasks.Task;


namespace Core.Interfaces;
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> ListAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T> GetByConditionAsync(Expression<Func<T, bool>> expression);
    Task<IReadOnlyList<T>> ListAllByConditionAsync(Expression<Func<T, bool>> expression);
}