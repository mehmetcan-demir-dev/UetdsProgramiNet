using System.Linq.Expressions;
using UetdsProgramiNet.Entities;

namespace UetdsProgramiNet.Services
{
    public interface IService<Entity> where Entity : BaseEntity
    {
        Task<Entity> AddAsync(Entity data);
        Task<IEnumerable<Entity>> AddRangeAsync(IEnumerable<Entity> datas);
        Task<Entity> GetByIdAsync(int id);
        Task<IEnumerable<Entity>> GetAllAsync();
        //Task<IEnumerable<Entity>> GetAllFilteredAsync(Expression<Func<Entity, bool>> whereEx, QueryFiltersDto filters);
        Task<bool> RemoveAsync(int id);
        Task<bool> RemoveRangeAsync(IEnumerable<string> ids);
        Task<Entity> UpdateAsync(Entity data);
        Task<IEnumerable<Entity>> UpdateRangeAsync(IEnumerable<Entity> datas);
        Task<bool> AnyAsync(Expression<Func<Entity, bool>> expression);
        Task<IEnumerable<Entity>> Where(Expression<Func<Entity, bool>> expression);
        int Count(Expression<Func<Entity, bool>>? expression = null);
        Task<bool> PersistentRemoveAsync(int id);
        Task<bool> PersistentRemoveRangeAsync(IEnumerable<string> ids);
    }
}
