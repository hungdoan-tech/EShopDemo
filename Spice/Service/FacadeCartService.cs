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

        private readonly CartService _cartService;
        private readonly IPaymentService _paymentService;
        private readonly IEmailService _emailService;
        private readonly ISessionService _sessionService;        

        public CartFacadeService(IUnitOfWork unitOfWork, 
                                IHttpContextAccessor httpContextAccessor,
                                IEmailService emailService)
        {
            this._unitOfWork = unitOfWork;
            this._httpContextAccessor = httpContextAccessor;
            this._emailService = emailService;

            this._cartService = new CartService(_unitOfWork, _httpContextAccessor, _sessionService);
            this._paymentService = new PaymentService();            
            this._sessionService = new SessionService(_httpContextAccessor);
        }

        public void SaveObjectsToDB(OrderDetailsCart detailCart, Claim claim)
        {
            this._cartService.SaveOrderHeaderAddOrderDetail(detailCart,claim);
        }

        public void ApplyCoupon(OrderDetailsCart detailCart)
        {
            this._cartService.ApplyCoupon(detailCart);
        }

        public void ChargeMoney(OrderDetailsCart detailCart,string stripeToken)
        {
            this._paymentService.ChargeStripe(detailCart, stripeToken);
        }

        public void SendCommittedEmail(OrderDetailsCart detailCart, Claim claim)
        {
            this._emailService.SendSummittedMail(detailCart, claim);
        }

        public void CreateOrderHeaderBeforeSumary(OrderDetailsCart detailCart, Claim claim)
        {
            this._cartService.CreateOrderHeaderBeforeSumary(detailCart, claim);
        }

        public void CheckCouponBeforeSumary(OrderDetailsCart detailCart)
        {
            this._cartService.CheckCouponBeforeSumary(detailCart);
        }

        public void ClearSession()
        {
            this._sessionService.Clear();
        }

        public void PrepareForIndexCart(OrderDetailsCart detailCart)
        {
            this._cartService.PrepareForIndexCart(detailCart);
        }

        public Boolean CheckCurrentItemQuantity(int cartId)
        {
            return this._cartService.CheckCurrentItemQuantity(cartId);
        }
    }
}
