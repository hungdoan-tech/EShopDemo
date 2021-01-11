using Spice.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Spice.Service.ServiceInterfaces
{
    interface ICartService
    {
        OrderDetailsCart SaveOrderHeaderAddOrderDetail(OrderDetailsCart detailCart, Claim claim);

        OrderDetailsCart ApplyCoupon(OrderDetailsCart detailCart);

        OrderDetailsCart CheckCouponBeforeSumary(OrderDetailsCart detailCart);

        OrderDetailsCart CreateOrderHeaderBeforeSumary(OrderDetailsCart detailCart, Claim claim);

        OrderDetailsCart PrepareForIndexCart(OrderDetailsCart detailCart);

        bool CheckCurrentItemQuantity(int cartId);        
    }
}
