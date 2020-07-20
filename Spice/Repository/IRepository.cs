using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Repository
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> ReadAll();
        TEntity ReadOne(int? id);
        void Create(TEntity entity);
        void Update(TEntity entity);
        void Delete(int? id);
        void Delete(TEntity entity);
    }
}
