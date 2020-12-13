using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Repository
{
    public class NewsRepository : GenericRepository<News>, INewsRepository
    {
        public NewsRepository(ApplicationDbContext context) : base(context)
        {

        }

        public IEnumerable<News> ReadAllCouponOrNews()
        {
            return this.dbSet.Include(m => m.MenuItem).Where(m => m.Type == "1" || m.Type == "2");
        }

        public IEnumerable<News> ReadAllNewsIncludeMenuItem()
        {
            return this.dbSet.Include(a => a.MenuItem);
        }

        public News ReadOneNewsIncludeMenuItem(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException($"Key value must not be null");
            }
            try
            {
                return this.dbSet.Include(a => a.MenuItem).SingleOrDefault(a => a.Id == id);
            }
            catch (Exception)
            {
                throw new Exception("Couldn't retrieve entities");
            }
        }
    }
}
