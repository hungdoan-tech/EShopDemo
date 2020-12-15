using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.ViewModels
{
    public class MenuItemsAndQuantity
    {
        public MenuItem Item { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to {1}")]
        public int Quantity { get; set; }
        public News News { get; set; }
        public FavoritedProduct FavoritedProduct { get; set; }
        public Rating CustomerRating { get; set; }
        public ProductStar ProductStar { get; set; }
        public List<Rating> ExistedRatings { get; set; }
    }
}
