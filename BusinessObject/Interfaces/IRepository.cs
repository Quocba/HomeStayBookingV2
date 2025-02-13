using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync<TKey>(TKey id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveAsync();
        Task<IQueryable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);
        IQueryable<T> FindWithInclude(params Expression<Func<T, object>>[] includes);
        Task AddRangeAsync(IEnumerable<T> entities);
        void DeleteRange(IEnumerable<T> entities);
    }
}
