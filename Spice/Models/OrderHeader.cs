using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser Customer { get; set; }
        public DateTime OrderDate { get; set; }
        public double OrderTotalOriginal { get; set; }
        [DisplayFormat(DataFormatString = "{0:C}")]
        public double OrderTotal { get; set; }
        public string CouponCode { get; set; }
        public double CouponCodeDiscount { get; set; }
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Comments { get; set; }
        public string PickupName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        #nullable enable
        public string? CustomerSignature { get; set; }
        #nullable disable
        public string ShipperId { get; set; }
        public virtual ApplicationUser Shipper { get; set; }
        public string TransactionId { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; }

        public OrderHeader()
        {

        }
        public OrderHeader(string userId, double orderTotalOriginal, double orderTotal, string pickupName, string phoneNumber, string email, string streetAddress, string city)
        {
            UserId = userId;
            OrderTotalOriginal = orderTotalOriginal;
            OrderTotal = orderTotal;
            PickupName = pickupName;
            PhoneNumber = phoneNumber;
            Email = email;
            StreetAddress = streetAddress;
            City = city;
        }
    }
}
