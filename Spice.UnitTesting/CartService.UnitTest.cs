using Microsoft.AspNetCore.Http;
using Moq;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service;
using Spice.Service.ServiceInterfaces;
using System;
using Xunit;
using static Spice.Models.Coupon;

namespace Spice.UnitTesting
{
    public class CartServiceTest
    {
        private readonly CartService _cartService;
        private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
        private readonly Mock<ISessionService> _sessionService = new Mock<ISessionService>();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new Mock<IHttpContextAccessor>();
        public CartServiceTest()
        {
            this._cartService = new CartService(_unitOfWork.Object, _httpContextAccessor.Object, _sessionService.Object);
        }

        [Fact]
        public void PrepareForIndexCart_ValidCart_ShouldChageItemsInCart()
        {
            //Arrage
            OrderDetailsCart orderDetailsCart = new OrderDetailsCart()
            {
                ListCart = new System.Collections.Generic.List<MenuItemsAndQuantity>()
                {
                    new MenuItemsAndQuantity()
                    {
                        Item = new MenuItem()
                        {
                            Name = "This must be the greatest name ever",
                            Description = "Some thing lazy",
                            CategoryId = 1,
                            SubCategoryId = 1
                        },
                        Quantity = 4
                    }
                    ,
                    new MenuItemsAndQuantity()
                    {
                        Item = new MenuItem()
                        {
                            Name = "This must 2nd fool name",
                            Description = "Some thing lazy or 100 character",
                            CategoryId = 2,
                            SubCategoryId = 1
                        },
                        Quantity = 5
                    }
                },
                OrderHeader = new Models.OrderHeader()
                {
                    OrderTotalOriginal = 375,
                    OrderTotal = 375
                }
            };
            Coupon coupon = new Coupon()
            {
                Name = "OFF15",
                Discount = 15,
                MinimumAmount = 50,
                IsActive = true,
                CouponType = "0"
            };
            this._sessionService.Setup(x => x.GetSession(It.IsAny<string>())).Returns("OFF15");
            this._unitOfWork.Setup(x => x.CouponRepository.FirstMatchName(It.IsAny<string>())).Returns(coupon);
            double expectedResult = orderDetailsCart.OrderHeader.OrderTotalOriginal * (100 - coupon.Discount) / 100;

            //Act
            OrderDetailsCart actualResult = _cartService.CheckCouponBeforeSumary(orderDetailsCart);

            //Assert
            Assert.Equal(expectedResult, actualResult.OrderHeader.OrderTotal);
        }


        [Fact]
        public void CheckCouponBeforeSumary_ValidCoupon_ShouldApplySaleToCart()
        {
            //Arrange
            OrderDetailsCart orderDetailsCart = new OrderDetailsCart()
            {
                ListCart = null,
                OrderHeader = new Models.OrderHeader()
                {
                    OrderTotalOriginal = 375,
                    OrderTotal = 375
                }
            };            
            Coupon coupon = new Coupon()
            {
                Name = "OFF15",
                Discount = 15,
                MinimumAmount = 50,
                IsActive = true,
                CouponType = "0"
            };
            this._sessionService.Setup(x => x.GetSession(It.IsAny<string>())).Returns("OFF15");
            this._unitOfWork.Setup(x => x.CouponRepository.FirstMatchName(It.IsAny<string>())).Returns(coupon);
            double expectedResult = orderDetailsCart.OrderHeader.OrderTotalOriginal * (100 - coupon.Discount) / 100;

            //Act
            OrderDetailsCart actualResult = _cartService.CheckCouponBeforeSumary(orderDetailsCart);

            //Assert
            Assert.Equal(expectedResult, actualResult.OrderHeader.OrderTotal);
        }
    }
}
