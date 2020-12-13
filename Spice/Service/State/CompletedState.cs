using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Repository;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;


namespace Spice.Service.State
{
    public class CompletedState : IOrderState
    {
        public void HandleRequest(IUnitOfWork _unitOfWork, IEmailSender _emailSender, int OrderId)
        {
            StateHandlingUtils.SendNotifyEmail(_unitOfWork, _emailSender, OrderId, SD.StatusCompleted, Message: " has been completed");
        }
    }
}
