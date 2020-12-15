using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class ProductStar
    {
        public int totalOneStar { get ; set; }
        public int totalTwoStar { get; set; }
        public int totalThreeStar { get; set; }
        public int totalFourStar { get; set; }
        public int totalFiveStar { get; set; }

        public double averageStar { get; set; }
    }
}
