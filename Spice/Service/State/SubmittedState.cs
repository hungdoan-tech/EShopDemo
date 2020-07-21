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
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.ReadOne(OrderId);
            orderHeader.Status = SD.StatusSubmitted;
            _unitOfWork.SaveChanges();
            _emailSender.SendEmailAsync(_unitOfWork.ApplicationUserRepository.ReadOneByStringID(orderHeader.UserId).Email, "Order number " + orderHeader.Id.ToString() + " Submit", "Order is Submited.");
        }
    }
}
