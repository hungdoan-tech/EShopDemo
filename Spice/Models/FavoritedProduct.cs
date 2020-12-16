using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class FavoritedProduct
    {
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual MenuItem MenuItem { get; set; }
        public string UserId { get; set; }
        public int ItemId { get; set; }
    }
}
