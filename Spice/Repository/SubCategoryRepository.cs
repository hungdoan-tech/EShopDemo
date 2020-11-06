using Spice.Data;
using Spice.Models;
using Spice.RepositoryInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Repository
{
    public class SubCategoryRepository : GenericRepository<SubCategory>, ISubCategoryRepository
    {
        public SubCategoryRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
