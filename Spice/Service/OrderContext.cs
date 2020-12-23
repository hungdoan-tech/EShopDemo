using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Repository;
using Spice.Service.ServiceInterfaces;

namespace Spice.Service.State
{
    public class OrderContext : IOrderContext
    {
        protected readonly IEmailService _emailService;
        private IOrderState state;

        public OrderContext(IEmailService emailService)
        {
            this._emailService = emailService;
        }

        public void SetState(IOrderState state)
        {
            this.state = state;
        }

        public void ApplyState(int OrderId)
        {
            this.state.HandleRequest(this._emailService, OrderId);
        }
    }
}
