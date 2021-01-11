using Spice.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.RepositoryInterface
{
    public interface IMenuItemRepository: IRepository<MenuItem>
    {
        IEnumerable<MenuItem> ReadAllIncludeCategoryAndSubCategory();
        IEnumerable<MenuItem> FilterMostPopularMenuItems();
        IEnumerable<MenuItem> FilterMostNewMenuItems();
        IEnumerable<MenuItem> FilterMostBestSellerMenuItems();
        MenuItem ReadOneIncludeCategoryAndSubCategory(int? id);
    }
}
