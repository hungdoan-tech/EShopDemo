using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models.Builder
{
    public interface IOrderHeaderBuilder
    {
        IOrderHeaderBuilder LinkUserID(string Id);
        IOrderHeaderBuilder OrderTotalOriginal(double OrderTotalOriginal);
        IOrderHeaderBuilder OderTotal(double OrderTotal);
        IOrderHeaderBuilder AddPickupName(string PickupName);
        IOrderHeaderBuilder AddPhoneNumber(string PhoneNumber);
        IOrderHeaderBuilder AddEmail(string Email);
        IOrderHeaderBuilder AddStreet(string StreetAdress);
        IOrderHeaderBuilder AddCity(string City);
        OrderHeader Build();
    }
}
