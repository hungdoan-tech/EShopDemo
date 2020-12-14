using Spice.Models.ViewModels;

namespace Spice.Service.ServiceInterfaces
{
    public interface IPaymentService
    {
        public OrderDetailsCart ChargeStripe(OrderDetailsCart detailCart, string stripeToken);
    }
}
