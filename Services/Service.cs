using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using UetdsProgramiNet.Entities;
using UetdsProgramiNet.Repositories;

namespace UetdsProgramiNet.Services
{
    public class Service<Entity> : IService<Entity> where Entity : BaseEntity
    {
        private readonly IGenericRepo<Entity> _repository;
        protected readonly IUnitOfWork _unitOfWork;
        public Service(IGenericRepo<Entity> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Entity> AddAsync(Entity data)
        {
            try
            {
                await _repository.AddAsync(data);
                await _unitOfWork.CommitAsync();
                return data;
            }
            catch (Exception)
            {
                _repository.EntityDetached(data);
                throw;
            }
        }

        public async Task<IEnumerable<Entity>> AddRangeAsync(IEnumerable<Entity> datas)
        {
            try
            {
                await _repository.AddRangeAsync(datas);
                await _unitOfWork.CommitAsync();
                return datas;
            }
            catch
            {
                _repository.EntitiesDetached(datas);
                throw;
            }
        }

        public async Task<Entity> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Entity>> GetAllAsync()
        {
            try
            {
                var entities = await _repository.GetAll().ToListAsync();
                return entities;
            }
            catch (Exception)
            {

                throw;
            }
        }

        //public async Task<IEnumerable<Entity>> GetAllFilteredAsync(Expression<Func<Entity, bool>> whereEx, QueryFiltersDto filters)
        //{
        //    try
        //    {
        //        return await _repository.GetAllFilteredAsync(whereEx, filters).ToListAsync();
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}

        public async Task<bool> RemoveAsync(int id)
        {

            try
            {
                var entity = await _repository.GetByIdAsync(id);
                entity.IsDeleted = true;
                _repository.Update(entity);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                var entity = await _repository.GetByIdAsync(id);
                _repository.EntityDetached(entity);
                throw;
            }
        }

        public async Task<bool> RemoveRangeAsync(IEnumerable<string> ids)
        {
            try
            {
                var entities = await _repository.Where(m => ids.Contains(m.Id.ToString())).ToListAsync();
                foreach (var entity in entities)
                {
                    entity.IsDeleted = true;
                }
                _repository.UpdateRange(entities);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                var entities = await _repository.Where(m => ids.Contains(m.Id.ToString())).ToListAsync();
                _repository.EntitiesDetached(entities);
                throw;
            }
        }

        public async Task<Entity> UpdateAsync(Entity data)
        {
            try
            {
                _repository.Update(data);
                await _unitOfWork.CommitAsync();
                return data;
            }
            catch (Exception)
            {
                _repository.EntityDetached(data);
                throw;
            }
        }
        public async Task<IEnumerable<Entity>> UpdateRangeAsync(IEnumerable<Entity> datas)
        {
            try
            {
                _repository.UpdateRange(datas);
                await _unitOfWork.CommitAsync();
                return datas;
            }
            catch (Exception)
            {
                _repository.EntitiesDetached(datas);
                throw;
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<Entity, bool>> expression)
        {
            bool anyEntity = await _repository.AnyAsync(expression);
            return anyEntity;
        }

        public async Task<IEnumerable<Entity>> Where(Expression<Func<Entity, bool>> expression)
        {
            try
            {
                var entities = await _repository.Where(expression).ToListAsync();
                return entities;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public int Count(Expression<Func<Entity, bool>>? expression = null)
        {
            return _repository.Count(expression);
        }

        public async Task<bool> PersistentRemoveAsync(int id)
        {
            try
            {
                var entity = await _repository.GetByIdAsync(id);
                _repository.Remove(entity);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                var entity = await _repository.GetByIdAsync(id);
                _repository.EntityDetached(entity);
                throw;
            }
        }

        public async Task<bool> PersistentRemoveRangeAsync(IEnumerable<string> ids)
        {
            try
            {
                var entities = await _repository.Where(m => ids.Contains(m.Id.ToString())).ToListAsync();
                _repository.RemoveRange(entities);
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                var entities = await _repository.Where(m => ids.Contains(m.Id.ToString())).ToListAsync();
                _repository.EntitiesDetached(entities);
                throw;
            }
        }
    }
}
