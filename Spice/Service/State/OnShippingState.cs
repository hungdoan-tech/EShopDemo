using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Models;
using Spice.Repository;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Service.State
{
    public class OnShippingState : IOrderState
    {
        public void HandleRequest(IUnitOfWork _unitOfWork, IEmailSender _emailSender, int OrderId)
        {
            StateStaticMethods.SendNotifyEmail(_unitOfWork, _emailSender, OrderId, SD.StatusReady, Message: " is on shipping");
        }
    }
}
