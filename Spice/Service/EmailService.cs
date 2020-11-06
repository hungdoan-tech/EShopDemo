using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Data;
using Spice.Models.ViewModels;
using Spice.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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
            _emailSender.SendEmailAsync(_unitOfWork.ApplicationUserRepository.ReadOneByStringID(claim.Value).Email, "Hello Customer, Order number " + detailCart.OrderHeader.Id.ToString() + " is created", "Order has been created");
        }
    }
}
