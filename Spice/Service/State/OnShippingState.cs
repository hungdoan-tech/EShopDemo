﻿using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Repository;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;


namespace Spice.Service.State
{
    public class OnShippingState : IOrderState
    {       
        public void HandleRequest(IEmailService _emailService, int OrderId)
        {
            _emailService.SendNotifyEmail(OrderId, SD.StatusReady, Message: " is on shipping");
        }
    }
}
