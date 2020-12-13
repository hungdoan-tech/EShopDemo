using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Repository;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;

namespace Spice.Service.State
{
    public class SubmittedState: IOrderState
    {
        void IOrderState.HandleRequest(IUnitOfWork _unitOfWork, IEmailSender _emailSender, int OrderId)
        {
            StateHandlingUtils.SendNotifyEmail(_unitOfWork, _emailSender, OrderId, SD.StatusSubmitted, Message: " is summitted");
        }
    }
}
