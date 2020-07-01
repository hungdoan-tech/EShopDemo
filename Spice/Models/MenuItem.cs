using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = " Price should be greater than ${1}")]
        public double Price { get; set; }
        public Boolean IsPublish { get; set; }
        // Quantity In repository
        [Range(1, int.MaxValue, ErrorMessage = " Quantity should be greater than 1")]
        public int Quantity { get; set; }
        public enum EColor { Red = 0, White = 1, Blue = 2, Black = 3, Yellow = 4, NA=5}
        public string Color { get; set; }
        public enum ETag { BestSeller = 0, New = 1, Popular = 2, NA=3 }
        public string Tag { get; set; }
        public DateTime PublishedDate { get; set; }


        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public int SubCategoryId { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
        public virtual ICollection<News> News { get; set; }
    }
}
