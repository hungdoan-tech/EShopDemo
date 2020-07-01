using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class NewsViewModel
    {
        public News News { get; set; }
        public IEnumerable<MenuItem> MenuItems { get; set; }
        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
    }
}
