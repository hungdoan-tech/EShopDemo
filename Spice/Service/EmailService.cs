using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Models.ViewModels;
using Spice.Repository;
using System.Security.Claims;

namespace Spice.Service
{
    public class EmailService
    {
        public IEmailSender _emailSender { get; set; }
        private readonly IUnitOfWork _unitOfWork;
        public EmailService(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }

        public void SendMailSummitted(OrderDetailsCart detailCart, Claim claim)
        {
            string receiverEmail = _unitOfWork.ApplicationUserRepository.ReadOneByStringID(claim.Value).Email;
            string title = "Hello Customer, Order number " + detailCart.OrderHeader.Id.ToString() + " is created";
            string content = "Order has been created";
            _emailSender.SendEmailAsync(receiverEmail, title, content);
        }
    }
}
