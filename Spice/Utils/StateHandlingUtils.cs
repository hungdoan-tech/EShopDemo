using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Models;
using Spice.Repository;

namespace Spice.Utility
{
    public static class StateHandlingUtils
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
