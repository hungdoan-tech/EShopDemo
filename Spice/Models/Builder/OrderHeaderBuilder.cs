using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.Builder
{
    public class OrderHeaderBuilder: IOrderHeaderBuilder
    {
        private string userId;
        private double orderTotalOriginal;
        private double orderTotal;
        private string pickupName;
        private string phoneNumber;
        private string email;
        private string streetAddress;
        private string city;

        public IOrderHeaderBuilder LinkUserID(string Id)
        {
            this.userId = Id;
            return this;
        }

        public IOrderHeaderBuilder OrderTotalOriginal(double OrderTotalOriginal)
        {
            this.orderTotalOriginal = OrderTotalOriginal;
            return this;
        }

        public IOrderHeaderBuilder OderTotal(double OrderTotal)
        {
            this.orderTotal = OrderTotal;
            return this;
        }

        public IOrderHeaderBuilder AddPickupName(string PickupName)
        {
            this.pickupName = PickupName;
            return this;
        }

        public IOrderHeaderBuilder AddPhoneNumber(string PhoneNumber)
        {
            this.phoneNumber = PhoneNumber;
            return this;
        }

        public IOrderHeaderBuilder AddEmail(string Email)
        {
            this.email = Email;
            return this;
        }

        public IOrderHeaderBuilder AddStreet(string StreetAdress)
        {
            this.streetAddress = StreetAdress;
            return this;
        }

        public IOrderHeaderBuilder AddCity(string City)
        {
            this.city = City;
            return this;
        }

        public OrderHeader Build()
        {
            return new OrderHeader(this.userId, this.orderTotalOriginal, this.orderTotal, this.pickupName, this.phoneNumber, this.email, this.streetAddress, this.city);
        }
    }
}
