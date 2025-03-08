using System.Linq.Expressions;

namespace UetdsProgramiNet.Repositories
{
    public interface IGenericRepo<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        IQueryable<T> GetAll();
        //IQueryable<T> GetAllFilteredAsync(Expression<Func<T, bool>> whereEx, QueryFiltersDto filters);
        IQueryable<T> Where(Expression<Func<T, bool>> expression);

        int Count(Expression<Func<T, bool>>? expression = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void EntityDetached(T entity);
        void EntitiesDetached(IEnumerable<T> entities);
    }
}
