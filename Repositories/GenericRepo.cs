using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace UetdsProgramiNet.Repositories
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepo(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.AnyAsync(expression);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }

        //public IQueryable<T> GetAllFilteredAsync(Expression<Func<T, bool>> whereEx, QueryFiltersDto filters)
        //{
        //    if (filters.TakeValue > 0)
        //        return _dbSet.Where(whereEx).Skip(filters.SkipValue).Take(filters.TakeValue).AsNoTracking().AsQueryable();
        //    else
        //        return _dbSet.Where(whereEx).Skip(filters.SkipValue).AsNoTracking().AsQueryable();
        //}

        public async Task<T> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression).AsNoTracking().AsQueryable();
        }

        public int Count(Expression<Func<T, bool>>? expression = null)
        {
            return _dbSet.Count();
        }
        public void EntityDetached(T entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        public void EntitiesDetached(IEnumerable<T> entities)
        {
            _context.Entry(entities).State = EntityState.Detached;
        }
    }
}
