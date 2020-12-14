using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Repository;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;

namespace Spice.Service.State
{
    public class PreparedState : IOrderState
    {       
        public void HandleRequest(IEmailService _emailService, int OrderId)
        {
            _emailService.SendNotifyEmail(OrderId, SD.StatusInProcess, Message: " is prepared at our repository");
        }
    }
}
