using BusinessObject.Interfaces;
using DataAccess.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class Repository<T>(ApplicationDbContext _context) : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbSet = _context.Set<T>();

        public async virtual Task<T> GetByIdAsync<TKey>(TKey id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async virtual Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async virtual Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async virtual Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        public async virtual Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async virtual Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IQueryable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public IQueryable<T> FindWithInclude(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query;
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
