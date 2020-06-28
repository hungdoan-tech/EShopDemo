using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class NewsViewModel
    {
        public News News { get; set; }
        public IEnumerable<NewsCategory> NewsCategory { get; set; }
    }
}
