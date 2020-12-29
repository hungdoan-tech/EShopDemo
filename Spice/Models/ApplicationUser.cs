using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public virtual ICollection<OrderHeader> CustomerOrderHeaders { get; set; }
        public virtual ICollection<OrderHeader> ShipperOrderHeaders { get; set; }
        public virtual ICollection<ImportHistory> ImportHistories { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<FavoritedProduct> FavoritedProducts { get; set; }
    }
}
