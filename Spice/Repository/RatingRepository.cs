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
    public class RatingRepository : GenericRepository<Rating>, IRatingRepository
    {
        public RatingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<Rating> ReadRatingIncludeMenuItem(int id)
        {
            return this.dbSet.Include(a => a.MenuItem).Where(b => b.MenuItem.Id == id);
        }
    }
}
