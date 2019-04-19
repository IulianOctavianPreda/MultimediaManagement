using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MultimediaManagement.Repository
{
    public interface ITemplateRepository<TEntity> : IDisposable
    {
        Task<TEntity> Get(Guid id);
        Task<IEnumerable<TEntity>> GetAll();
        Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate, int skip, int take);
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        bool Any(Expression<Func<TEntity, bool>> predicate);

        void Commit();
    }
}
