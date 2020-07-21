using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Service.State
{
    public class OrderContext
    {
        protected IUnitOfWork _unitOfWork;
        protected IEmailSender _emailSender;
        private IOrderState state;

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
