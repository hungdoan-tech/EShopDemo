using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;

namespace Spice.Repository
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context;

        internal DbSet<TEntity> dbSet;
        public GenericRepository(ApplicationDbContext context)
        {
            this._context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> ReadAll()
        {
            try
            {
                return this.dbSet;
            }
            catch (Exception)
            {
                throw new Exception("Couldn't retrieve entities");
            }
        }
        public virtual TEntity ReadOne(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException($"Key value must not be null");
            }
            try
            {
                return this.dbSet.Find(id);
            }
            catch (Exception)
            {
                throw new Exception("Couldn't retrieve entities");
            }
        }

        public virtual async Task<TEntity> ReadOneAsync(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException($"Key value must not be null");
            }
            try
            {
                return await this.dbSet.FindAsync(id);
            }
            catch (Exception)
            {
                throw new Exception("Couldn't retrieve entities");
            }
        }

        public virtual void Create(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(TEntity)} entity must not be null");
            }

            try
            {
                this._context.Add(entity);
            }
            catch (Exception)
            {
                throw new Exception($"{nameof(entity)} could not be saved");
            }
        }
        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(TEntity)} entity must not be null");
            }

            try
            {
                await this._context.AddAsync(entity);
                return entity;
            }
            catch (Exception)
            {
                throw new Exception($"{nameof(entity)} could not be saved");
            }
        }

        public virtual void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(TEntity)} entity must not be null");
            }

            try
            {
                this._context.Update(entity);
            }
            catch (Exception)
            {
                throw new Exception($"{nameof(entity)} could not be updated");
            }
        }

        public virtual void Delete(int? id)
        {
            try
            {
                TEntity entityToDelete = ReadOne(id);
                Delete(entityToDelete);
            }
            catch (Exception)
            {
                throw new Exception($"{nameof(TEntity)} could not be deleted");
            }
        }

        public virtual void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(TEntity)} entity must not be null");
            }

            try
            {
                if (this._context.Entry(entity).State == EntityState.Detached)
                {
                    dbSet.Attach(entity);
                }
                dbSet.Remove(entity);
            }

            catch (Exception)
            {
                throw new Exception($"{nameof(TEntity)} could not be deleted");
            }
        }
        public virtual IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            // Query là 1 dạng IQueryable, chỉ được thực thi khi cần giá trị list
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Tiếp theo, nó sẽ kèm theo các property cần thiết khi người dùng chỉ định
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            // Sau cùng, nó thực thi bằng cách translate thành câu lệnh SQL và gọi xuống database
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }
        public virtual void SaveChanges()
        {
           this._context.SaveChanges();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await this._context.SaveChangesAsync();
        }
    }
}
