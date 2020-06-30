using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{

    public class ProductsListViewModel
    {
        public List<MenuItem> Products { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public List<Category> Categories { get; set; }
        public List<SubCategory> SubCategories { get; set; }
    }
}
