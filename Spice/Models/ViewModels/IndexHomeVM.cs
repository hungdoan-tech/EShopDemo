using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class IndexHomeVM
    {
        public IEnumerable<MenuItem> ListPopularMenuItem { get; set; }
        public IEnumerable<MenuItem> ListNewMenuItem { get; set; }
        public IEnumerable<MenuItem> ListBestSellerMenuItem { get; set; }
        public IEnumerable<News> ListNews { get; set; }
    }
}
