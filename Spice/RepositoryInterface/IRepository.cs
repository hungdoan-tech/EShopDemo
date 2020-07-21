using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Spice.RepositoryInterface
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> ReadAll();
        TEntity ReadOne(int? id);
        Task<TEntity> ReadOneAsync(int? id);
        void Create(TEntity entity);
        Task<TEntity> CreateAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(int? id);
        void Delete(TEntity entity);
        IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = "");
        void SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
