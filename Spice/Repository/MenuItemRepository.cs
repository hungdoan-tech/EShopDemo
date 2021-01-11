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

        public IEnumerable<MenuItem> FilterMostBestSellerMenuItems()
        {
            return this.dbSet.Where(a => a.Tag == "0" && a.IsPublish == true)
                                                    .OrderByDescending(a => a.Id)
                                                    .Take(2).Include(a => a.Category)
                                                    .Include(a => a.SubCategory);
        }

        public IEnumerable<MenuItem> FilterMostNewMenuItems()
        {
            return this.dbSet.Where(a => a.Tag == "1" && a.IsPublish == true)
                                                    .OrderByDescending(a => a.Id)
                                                    .Take(6).Include(a => a.Category)
                                                    .Include(a => a.SubCategory);
        }

        public IEnumerable<MenuItem> FilterMostPopularMenuItems()
        {
            return this.dbSet.Where(a => a.Tag == "2" && a.IsPublish == true)
                                                         .OrderByDescending(a => a.Id)
                                                         .Take(6)
                                                         .Include(a => a.Category)
                                                         .Include(a => a.SubCategory);
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
