using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;
using System.Linq;
using System.Security.Claims;

namespace Spice.Service
{
    public class EmailService : IEmailService
    {
        public IEmailSender _emailSender { get; set; }
        private readonly IUnitOfWork _unitOfWork;
        public EmailService(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        public void SendSummittedMail(OrderDetailsCart detailCart, Claim claim)
        {
            string receiverEmail = _unitOfWork.ApplicationUserRepository.ReadOneByStringID(claim.Value).Email;
            string title = "Hello Customer, Order number " + detailCart.OrderHeader.Id.ToString() + " is created";
            string content = "Order has been created";
            _emailSender.SendEmailAsync(receiverEmail, title, content);
        }

        public void SendNotifyEmail(int OrderId, string OrderStatus, string Message)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.ReadOne(OrderId);
            orderHeader.Status = OrderStatus;
            _unitOfWork.SaveChanges();
            _emailSender.SendEmailAsync(_unitOfWork.ApplicationUserRepository.ReadOneByStringID(orderHeader.UserId).Email,
                                        "Order number " + orderHeader.Id.ToString() + Message,
                                        "Order " + orderHeader.Id.ToString() + Message);
        }

        public void SendCanceledMail(int OrderId)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.ReadOne(OrderId);
            orderHeader.Status = SD.StatusCancelled;
            try
            {
                var ListOrderDetail = _unitOfWork.OrderDetailRepository.Get(a => a.OrderId == orderHeader.Id).ToList();
                foreach (var orderDetail in ListOrderDetail)
                {
                    _unitOfWork.MenuItemRepository.ReadOne(orderDetail.MenuItemId).Quantity += orderDetail.Count;
                }
                _unitOfWork.OrderDetailRepository.DeleteRange(ListOrderDetail);
                _unitOfWork.OrderHeaderRepository.Delete(orderHeader);
                _unitOfWork.SaveChanges();
            }
            catch
            {
                _unitOfWork.Dispose();
            }
            _emailSender.SendEmailAsync(_unitOfWork.ApplicationUserRepository.ReadOneByStringID(orderHeader.UserId).Email, 
                                       "Order number " + orderHeader.Id.ToString() + " has been cancelled", 
                                       "Order " + orderHeader.Id.ToString() + "has been cancelled.");
        }
    }
}
