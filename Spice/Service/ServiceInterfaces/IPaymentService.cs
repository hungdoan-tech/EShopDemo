using Spice.Models.ViewModels;

namespace Spice.Service.ServiceInterfaces
{
    public interface IPaymentService
    {
        public OrderDetailsCart Charge(OrderDetailsCart detailCart, string stripeToken);
    }
}
