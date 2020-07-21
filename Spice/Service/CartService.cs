using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Spice.Extensions;
using Spice.Repository;
using Spice.Models.Builder;

namespace Spice.Service
{
    public class CartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        public OrderDetailsCart SaveOrderHeaderAddOrderDetail(OrderDetailsCart detailCart, Claim claim)
        {
            detailCart.listCart = _httpContextAccessor.HttpContext.Session.Get<List<MenuItemsAndQuantity>>(SD.ssShoppingCart).ToList();
            detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            detailCart.OrderHeader.OrderDate = DateTime.Now;
            detailCart.OrderHeader.UserId = claim.Value;
            detailCart.OrderHeader.Status = SD.StatusSubmitted;

            List<OrderDetails> orderDetailsList = new List<OrderDetails>();
            _unitOfWork.OrderHeaderRepository.Create(detailCart.OrderHeader);
            _unitOfWork.SaveChanges();

            detailCart.OrderHeader.OrderTotalOriginal = 0;

            foreach (var item in detailCart.listCart)
            {
                item.Item = _unitOfWork.MenuItemRepository.ReadOne(item.Item.Id);
                _unitOfWork.MenuItemRepository.ReadOne(item.Item.Id).Quantity -= item.Quantity;
                if (_unitOfWork.MenuItemRepository.ReadOne(item.Item.Id).Quantity == 0)
                {
                    _unitOfWork.MenuItemRepository.ReadOne(item.Item.Id).IsPublish = false;
                }

                OrderDetails orderDetails = new OrderDetailBuilder()
                                            .AddName(item.Item.Name)
                                            .AddQuantity(item.Quantity)
                                            .MoreDescription(item.Item.Description)
                                            .CalPrice(item.Item.Price * item.Quantity)
                                            .LinkOrderID(detailCart.OrderHeader.Id)
                                            .LinkMenuItemID(item.Item.Id)
                                            .Build();                

                detailCart.OrderHeader.OrderTotalOriginal += orderDetails.Count * item.Item.Price;
                _unitOfWork.OrderDetailRepository.Create(orderDetails);
            }
            _unitOfWork.SaveChanges();
            return detailCart;
        }
        public OrderDetailsCart ApplyCoupon(OrderDetailsCart detailCart)
        {
            if (_httpContextAccessor.HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailCart.OrderHeader.CouponCode = _httpContextAccessor.HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = _unitOfWork.CouponRepository.FirstMatchName(detailCart.OrderHeader.CouponCode.ToLower());
                detailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.OrderHeader.OrderTotalOriginal);
            }
            else
            {
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotalOriginal;
            }
            detailCart.OrderHeader.CouponCodeDiscount = detailCart.OrderHeader.OrderTotalOriginal - detailCart.OrderHeader.OrderTotal;

            _httpContextAccessor.HttpContext.Session.Get<List<MenuItemsAndQuantity>>(SD.ssShoppingCart).Clear();
            _unitOfWork.SaveChanges();
            return detailCart;
        }
    }
}
