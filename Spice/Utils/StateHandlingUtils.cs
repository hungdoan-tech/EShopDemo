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
    public static class StateStaticMethods
    {
        public static void SendNotifyEmail(IUnitOfWork _unitOfWork, IEmailSender _emailSender, int OrderId, string OrderStatus, string Message)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.ReadOne(OrderId);
            orderHeader.Status = OrderStatus;
            _unitOfWork.SaveChanges();
            _emailSender.SendEmailAsync(_unitOfWork.ApplicationUserRepository.ReadOneByStringID(orderHeader.UserId).Email, "Order number " + orderHeader.Id.ToString() + Message, "Order " + orderHeader.Id.ToString() + Message);
        }
    }
}
