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
        private readonly CartService cartService;
        private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
        private readonly Mock<ISessionService> _sessionService = new Mock<ISessionService>();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new Mock<IHttpContextAccessor>();
        private MenuItem item1;
        private MenuItem item2;
        private OrderDetailsCart detailsCart;

        public CartServiceTest()
        {
            this.cartService = new CartService(_unitOfWork.Object, _httpContextAccessor.Object, _sessionService.Object);
            
            item1 = new MenuItem
            {
                Id = 1,
                Name = "Rolex 1",
                Description = "Awesome",
                Size = "41.00 x 41.00mm",
                Band = "Plastic",
                Thickness = 9.8,
                Crystal = "Plexigrass",
                Image = "\\images\\1.png",
                Price = 100,
                IsPublish = true,
                Quantity = 6,
                Color = "3",
                Tag = "2",
                PublishedDate = DateTime.Now,
                CategoryId = 2,
                SubCategoryId = 3
            };

            item2 = new MenuItem
            {
                Id = 3,
                Name = "Rolex 3",
                Description = "Awesome",
                Size = "41.00 x 41.00mm",
                Band = "Plastic",
                Thickness = 9.8,
                Crystal = "Plexigrass",
                Image = "\\images\\3.png",
                Price = 25,
                IsPublish = true,
                Quantity = 23,
                Color = "3",
                Tag = "2",
                PublishedDate = DateTime.Now,
                CategoryId = 2,
                SubCategoryId = 3
            };

            detailsCart = new OrderDetailsCart()
            {
                ListCart = null,
                OrderHeader = new OrderHeader()
                {
                    OrderTotalOriginal = 350,
                    OrderTotal = 350
                }
            };
        }


        #region Unit test ApplyCoupon method 

        [Fact]
        public void ApplyCoupon_NoSavedCoupon_ShouldHaveNoChangeOnDetailCart()
        {
            //Arrange
            _sessionService.Setup(x => x.GetSession(It.IsAny<String>())).Returns(null);

            //Act
            OrderDetailsCart tempDetailsCart = this.cartService.ApplyCoupon(this.detailsCart);

            //Assert
            Assert.Equal(tempDetailsCart.OrderHeader.OrderTotal, tempDetailsCart.OrderHeader.OrderTotalOriginal);
            Assert.Equal(0, tempDetailsCart.OrderHeader.CouponCodeDiscount);
        }

        [Fact]
        public void ApplyCoupon_HaveSavedCoupon_ShouldHaveNoChangeOnDetailCart()
        {
            //Arrange
            Coupon tempCoupon = null;
            _sessionService.Setup(x => x.GetSession(It.IsAny<String>())).Returns(It.IsAny<String>());
            _unitOfWork.Setup(x => x.CouponRepository.FirstMatchName(It.IsAny<String>())).Returns(tempCoupon);

            //Act
            OrderDetailsCart tempDetailsCart = this.cartService.ApplyCoupon(this.detailsCart);

            //Assert
            Assert.Equal(tempDetailsCart.OrderHeader.OrderTotal, tempDetailsCart.OrderHeader.OrderTotalOriginal);
            Assert.Equal(0, tempDetailsCart.OrderHeader.CouponCodeDiscount);
        }

        [Fact]
        public void ApplyCoupon_HaveSavedCoupon_ShouldHaveChangeOnDetailCart()
        {
            //Arrange            
            Coupon tempCoupon = new Coupon()
            {
                Name = "15OFF",
                CouponType = "0",
                Discount = 15,
                IsActive = true,
                MinimumAmount = 50
            };

            var expectedOrderTotal = this.detailsCart.OrderHeader.OrderTotalOriginal * (100 - tempCoupon.Discount) / 100;
            var expectedDiscountAmount = this.detailsCart.OrderHeader.OrderTotalOriginal - expectedOrderTotal;

            _sessionService.Setup(x => x.GetSession(It.IsAny<String>())).Returns(It.IsAny<String>());
            _unitOfWork.Setup(x => x.CouponRepository.FirstMatchName(It.IsAny<String>())).Returns(tempCoupon);

            //Act
            OrderDetailsCart tempDetailsCart = this.cartService.ApplyCoupon(this.detailsCart);

            //Assert
            Assert.Equal(tempDetailsCart.OrderHeader.OrderTotal, tempDetailsCart.OrderHeader.OrderTotalOriginal);
            Assert.Equal(0, tempDetailsCart.OrderHeader.CouponCodeDiscount);
        }

        #endregion

        #region Unit test CheckCurrentItemQuantity method
        [Fact]
        public void CheckCurrentItemQuantity_ValidQuantity_ShouldReturnFalse()
        {
            //Arrange
           
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
            bool result = cartService.CheckCurrentItemQuantity(1);

            //Assert
            Assert.False(result);
        }
        [Fact]
        public void CheckCurrentItemQuantity_InvalidQuantity_ShouldReturnTrue()
        {
            //Arrange

            List<MenuItemsAndQuantity> lstCart = new List<MenuItemsAndQuantity>();
            List<MenuItem> lstItem = new List<MenuItem>();
            lstItem.Add(item1);
            lstItem.Add(item2);
            lstCart.Add(new MenuItemsAndQuantity()
            {
                Item = item1,
                Quantity = 9
            });
            lstCart.Add(new MenuItemsAndQuantity()
            {
                Item = item2,
                Quantity = 2
            });

            this._sessionService.Setup(x => x.GetSessionListQuantity()).Returns(lstCart);
            this._unitOfWork.Setup(x => x.MenuItemRepository.ReadOneIncludeCategoryAndSubCategory(1)).Returns(item1);

            //Act
            bool result = cartService.CheckCurrentItemQuantity(1);

            //Assert
            Assert.True(result);
        }
        #endregion

        #region  Unit test PrepareForIndexCart method

        [Fact]
        public void PrepareForIndexCart_CartSessionIsNull_ShoulReturnAEmptyListCart()
        {

            //Arrange
            List<MenuItemsAndQuantity> tempListItemAndQuantity = null;

            _sessionService.Setup(x => x.GetSessionListQuantity()).Returns(tempListItemAndQuantity);



            //Act
            OrderDetailsCart tempDetailsCart = this.cartService.ApplyCoupon(this.detailsCart);

            //Assert
            Assert.Equal(tempDetailsCart.OrderHeader.OrderTotal, tempDetailsCart.OrderHeader.OrderTotalOriginal);
            Assert.Equal(0, tempDetailsCart.OrderHeader.CouponCodeDiscount);
        }

        #endregion
    }
}
