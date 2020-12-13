using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service.ServiceInterfaces;
using System;
using System.Security.Claims;


namespace Spice.Service
{
    public class CartFacadeService: IFacadeCartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailSender _emailSender;

        private CartService cartService;
        private PaymentService paymentService;
        private EmailService emailService;
        private SessionService sessionService;

        public CartFacadeService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _emailSender = emailSender;

            this.cartService = new CartService(_unitOfWork, _httpContextAccessor);
            this.paymentService = new PaymentService();
            this.emailService = new EmailService(_unitOfWork, _emailSender);
            this.sessionService = new SessionService(_httpContextAccessor);
        }

        public void SaveObjectsToDB(OrderDetailsCart detailCart, Claim claim)
        {
            this.cartService.SaveOrderHeaderAddOrderDetail(detailCart,claim);
        }

        public void ApplyCoupon(OrderDetailsCart detailCart)
        {
            this.cartService.ApplyCoupon(detailCart);
        }

        public void ChargeMoney(OrderDetailsCart detailCart,string stripeToken)
        {
            this.paymentService.Charge(detailCart, stripeToken);
        }

        public void SendEmailCommitted(OrderDetailsCart detailCart, Claim claim)
        {
            this.emailService.SendMailSummitted(detailCart, claim);
        }

        public void CreateOrderHeaderBeforeSumary(OrderDetailsCart detailCart, Claim claim)
        {
            this.cartService.CreateOrderHeaderBeforeSumary(detailCart, claim);
        }
        public void CheckCouponBeforeSumary(OrderDetailsCart detailCart)
        {
            this.cartService.CheckCouponBeforeSumary(detailCart);
        }
        public void ClearSession()
        {
            this.sessionService.Clear();
        }

        public void PrepareForIndexCart(OrderDetailsCart detailCart)
        {
            this.cartService.PrepareForIndexCart(detailCart);
        }

        public Boolean CheckCurrentItemQuantity(int cartId)
        {
            return this.cartService.CheckCurrentItemQuantity(cartId);
        }
    }
}
