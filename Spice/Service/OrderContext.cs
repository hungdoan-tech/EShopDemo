using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Repository;
using Spice.Service.ServiceInterfaces;

namespace Spice.Service.State
{
    public class OrderContext
    {
        protected IUnitOfWork _unitOfWork;
        protected IEmailSender _emailSender;
        private IOrderState state;

        public OrderContext(IUnitOfWork unitOfWork, IEmailSender emailSender, IOrderState state)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            this.state = state;
        }

        public void SetState(IOrderState state)
        {
            this.state = state;
        }

        public void ApplyState(int OrderId)
        {
            this.state.HandleRequest(_unitOfWork,_emailSender, OrderId);
        }
    }
}
