using Spice.Models.ViewModels;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;
using Stripe;
using System;

namespace Spice.Service
{
    public class PaymentService: IPaymentService
    {
        public OrderDetailsCart ChargeStripe(OrderDetailsCart detailCart, string stripeToken)
        {
            var options = new ChargeCreateOptions
            {
                Amount = Convert.ToInt32(detailCart.OrderHeader.OrderTotal * 100),
                Currency = "USD",
                Description = "Order ID : " + detailCart.OrderHeader.Id,
                Source = stripeToken
            };
            var service = new ChargeService();
            Charge charge = service.Create(options);

            if (charge.BalanceTransactionId == null)
            {
                detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            else
            {
                detailCart.OrderHeader.TransactionId = charge.BalanceTransactionId;
            }

            if (charge.Status.ToLower() == "succeeded")
            {
               
                detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
            }
            else
            {
                detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
            }
            return detailCart;
        }
    }
}
