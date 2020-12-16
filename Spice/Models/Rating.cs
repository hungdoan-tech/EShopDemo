using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int RatingStar { get; set; }
        public enum RatingNumber { One, Two, Three, Four, Five};
        public DateTime PublishedDate { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public int? MenuItemId { get; set; }
        public virtual MenuItem MenuItem { get; set; }
    }
}

