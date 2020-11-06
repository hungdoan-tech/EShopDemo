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
    public class MenuItemRepository : GenericRepository<MenuItem>, IMenuItemRepository
    {
        public MenuItemRepository(ApplicationDbContext context) : base(context)
        {

        }

        public IEnumerable<MenuItem> ReadAllIncludeCategoryAndSubCategory()
        {
            return this.dbSet.Include(m => m.Category).Include(m => m.SubCategory);
        }

        public MenuItem ReadOneIncludeCategoryAndSubCategory(int? id)
        {
            return this.dbSet.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefault(m => m.Id == id);
        }
    }
}
