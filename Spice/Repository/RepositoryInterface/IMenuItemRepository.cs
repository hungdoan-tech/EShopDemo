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
        MenuItem ReadOneIncludeCategoryAndSubCategory(int? id);
    }
}
