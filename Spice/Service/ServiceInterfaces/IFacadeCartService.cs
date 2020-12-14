using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Service.ServiceInterfaces
{
    public interface IFacadeCartService
    {
        void SaveObjectsToDB(OrderDetailsCart detailCart, Claim claim);
        void ApplyCoupon(OrderDetailsCart detailCart);
        void ChargeMoney(OrderDetailsCart detailCart, string stripeToken);
        void SendCommittedEmail(OrderDetailsCart detailCart, Claim claim);
        void CreateOrderHeaderBeforeSumary(OrderDetailsCart detailCart, Claim claim);
        void CheckCouponBeforeSumary(OrderDetailsCart detailCart);
        void PrepareForIndexCart(OrderDetailsCart detailCart);
        Boolean CheckCurrentItemQuantity(int cartId);
        void ClearSession();
    }
}
