using System;
using System.Collections.Generic;
using System.Linq;
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
        void SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
