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
    public class SubmittedState: IOrderState
    {
        void IOrderState.HandleRequest(IUnitOfWork _unitOfWork, IEmailSender _emailSender, int OrderId)
        {
            StateStaticMethods.SendNotifyEmail(_unitOfWork, _emailSender, OrderId, SD.StatusSubmitted, Message: " is summitted");
        }
    }
}
