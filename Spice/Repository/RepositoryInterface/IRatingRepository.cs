using Spice.Models;
using Spice.Repository;
using Spice.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Repository.RepositoryInterface
{
    public interface IRatingRepository : IRepository<Rating>
    {
        public IEnumerable<Rating> ReadRatingIncludeMenuItem(int id);
    }
    
}
