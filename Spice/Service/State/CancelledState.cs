using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Models;
using Spice.Repository;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;
using System.Linq;


namespace Spice.Service.State
{
    public class CancelledState : IOrderState
    {
        public void HandleRequest(IEmailService _emailService, int OrderId)
        {
            _emailService.SendCanceledMail(OrderId);
        }
    }
}
