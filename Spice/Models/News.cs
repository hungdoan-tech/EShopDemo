using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Header { get; set; }
        public string Content { get; set; }
        public string Alias { get; set; }
        public DateTime PublishedDate { get; set; }
        public string ImageHeader { get; set; }
        public string Type { get; set; }
        public enum EType { IntergratedInItem = 0, Coupon = 1, News = 2 }
        public int? MenuItemId { get; set; }
        public virtual MenuItem MenuItem { get; set; }
    }
}
