using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class Coupon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CouponType { get; set; }
        public enum ECouponType { Percent = 0, Dollar = 1 }
        public double Discount { get; set; }
        public double MinimumAmount { get; set; }
        public bool IsActive { get; set; }
        public Coupon()
        {

        }
        public Coupon(int id, string name, string couponType, double discount, double minimumAmount, bool isActive)
        {
            Id = id;
            Name = name;
            CouponType = couponType;
            Discount = discount;
            MinimumAmount = minimumAmount;
            IsActive = isActive;
        }
    }
}
