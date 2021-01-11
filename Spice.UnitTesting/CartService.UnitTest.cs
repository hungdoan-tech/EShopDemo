using Microsoft.AspNetCore.Http;
using Moq;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service;
using Spice.Service.ServiceInterfaces;
using System;
using System.Collections.Generic;
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
        [Fact]
        public void CheckCurrentItemQuantity_ValidCheking_ReturnTrue()
        {
            //Arrange
            var item1 = new MenuItem { 
                Id = 1, Name = "Rolex 1", 
                Description = "Awesome", 
                Size = "41.00 x 41.00mm", 
                Band = "Plastic", 
                Thickness = 9.8, 
                Crystal = "Plexigrass", 
                Image = "\\images\\1.png", 
                Price = 100, IsPublish = true, 
                Quantity = 6, 
                Color = "3", 
                Tag = "2", 
                PublishedDate = DateTime.Now, 
                CategoryId = 2, 
                SubCategoryId = 3 
            };
            var item2 = new MenuItem { 
                Id = 3, 
                Name = "Rolex 3", 
                Description = "Awesome", 
                Size = "41.00 x 41.00mm", 
                Band = "Plastic", 
                Thickness = 9.8, 
                Crystal = "Plexigrass", 
                Image = "\\images\\3.png", 
                Price = 25, IsPublish = true, 
                Quantity = 23, 
                Color = "3", 
                Tag = "2", 
                PublishedDate = DateTime.Now, 
                CategoryId = 2, 
                SubCategoryId = 3 };
            List<MenuItemsAndQuantity> lstCart = new List<MenuItemsAndQuantity>();
            List<MenuItem> lstItem = new List<MenuItem>();
            lstItem.Add(item1);
            lstItem.Add(item2);
            lstCart.Add(new MenuItemsAndQuantity()
            {
                Item = item1,
                Quantity = 5
            });
            lstCart.Add(new MenuItemsAndQuantity()
            {
                Item = item2,
                Quantity = 2
            });

            this._sessionService.Setup(x => x.GetSessionListQuantity()).Returns(lstCart);
            this._unitOfWork.Setup(x => x.MenuItemRepository.ReadOneIncludeCategoryAndSubCategory(1)).Returns(item1);

            //Act
            bool result = _cartService.CheckCurrentItemQuantity(item1.Id);


            //Assert
            Assert.True(!result);
        }
    }
}
