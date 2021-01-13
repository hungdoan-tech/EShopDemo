using Microsoft.AspNetCore.Http;
using Moq;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service;
using Spice.Service.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using static Spice.Models.Coupon;

namespace Spice.UnitTesting
{
    public class CartServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWork = new Mock<IUnitOfWork>();
        private readonly Mock<ISessionService> _sessionService = new Mock<ISessionService>();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor = new Mock<IHttpContextAccessor>();

        private readonly CartService cartService;
        private List<MenuItemsAndQuantity> menuItemsAndQuantities;
        private MenuItem firstItem;
        private MenuItem secondItem;
        private OrderDetailsCart detailsCart;
        private Random random;

        public CartServiceTest()
        {
            this.cartService = new CartService(_unitOfWork.Object, _httpContextAccessor.Object, _sessionService.Object);
            this.random = new Random();

            firstItem = new MenuItem
            {
                Id = 1,
                Name = "Rolex 1",
                Description = this.RandomString(20),
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

            secondItem = new MenuItem
            {
                Id = 2,
                Name = "Rolex 3",
                Description = this.RandomString(80),
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

            this.menuItemsAndQuantities = new List<MenuItemsAndQuantity>();

            menuItemsAndQuantities.Add(
                new MenuItemsAndQuantity()
                {
                    Item = firstItem,
                    Quantity = 2
                });          
        }


        #region Unit test ApplyCoupon method 

        [Fact]
        public void ApplyCoupon_NoSavedCoupon_ShouldHaveNoChangesOnDetailCart()
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
        public void ApplyCoupon_HaveSavedCoupon_ShouldHaveNoChangesOnDetailCart()
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
        public void ApplyCoupon_HaveSavedCoupon_ShouldHaveChangesOnDetailCart()
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
            lstItem.Add(firstItem);
            lstItem.Add(secondItem);
            lstCart.Add(new MenuItemsAndQuantity()
            {
                Item = firstItem,
                Quantity = 5
            });
            lstCart.Add(new MenuItemsAndQuantity()
            {
                Item = secondItem,
                Quantity = 2
            });

            this._sessionService.Setup(x => x.GetSessionListQuantity()).Returns(lstCart);
            this._unitOfWork.Setup(x => x.MenuItemRepository.ReadOneIncludeCategoryAndSubCategory(1)).Returns(firstItem);

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
            lstItem.Add(firstItem);
            lstItem.Add(secondItem);
            lstCart.Add(new MenuItemsAndQuantity()
            {
                Item = firstItem,
                Quantity = 9
            });
            lstCart.Add(new MenuItemsAndQuantity()
            {
                Item = secondItem,
                Quantity = 2
            });

            this._sessionService.Setup(x => x.GetSessionListQuantity()).Returns(lstCart);
            this._unitOfWork.Setup(x => x.MenuItemRepository.ReadOneIncludeCategoryAndSubCategory(1)).Returns(firstItem);

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
            OrderDetailsCart tempDetailsCart = this.cartService.PrepareForIndexCart(this.detailsCart);

            //Assert
            Assert.Empty(tempDetailsCart.ListCart);
        }

        [Fact]
        public void PrepareForIndexCart_CartSessionIsEmpty_ShoulHaveNoChanges()
        {

            //Arrange
            List<MenuItemsAndQuantity> tempListItemAndQuantity = new List<MenuItemsAndQuantity>();
            _sessionService.Setup(x => x.GetSessionListQuantity()).Returns(tempListItemAndQuantity);

            //Act
            OrderDetailsCart tempDetailsCart = this.cartService.PrepareForIndexCart(this.detailsCart);

            //Assert
            Assert.Empty(tempDetailsCart.ListCart);
            Assert.Equal(tempDetailsCart.OrderHeader.OrderTotal, this.detailsCart.OrderHeader.OrderTotalOriginal);
        }

        [Fact]
        public void PrepareForIndexCart_CartSessionIsValidAndShortDesciption_ShoulHaveChangesInItems()
        {
            //Arrange                        

            this.detailsCart.OrderHeader.OrderTotalOriginal = 0;
            this.detailsCart.OrderHeader.OrderTotal = 0;
            double expectedOrderTotal = 0;
            foreach (var item in this.menuItemsAndQuantities)
            {
                expectedOrderTotal += item.Item.Price * item.Quantity;
            }

            _sessionService.Setup(x => x.GetSessionListQuantity()).Returns(this.menuItemsAndQuantities);
            _unitOfWork.Setup(x => x.MenuItemRepository.ReadOne(It.IsAny<int>())).Returns(menuItemsAndQuantities.First(x=>x.Item.Id == 1).Item);

            //Act
            OrderDetailsCart tempDetailsCart = this.cartService.PrepareForIndexCart(this.detailsCart);

            //Assert
            Assert.Equal(tempDetailsCart.OrderHeader.OrderTotalOriginal, expectedOrderTotal);
        }

        [Fact]
        public void PrepareForIndexCart_CartSessionIsValidAndLongDesciption_ShoulHaveChangesInItems()
        {
            //Arrange                        

            this.detailsCart.OrderHeader.OrderTotalOriginal = 0;
            this.detailsCart.OrderHeader.OrderTotal = 0;
            double expectedOrderTotal = 0;
            foreach (var item in this.menuItemsAndQuantities)
            {
                expectedOrderTotal += item.Item.Price * item.Quantity;
            }

            _sessionService.Setup(x => x.GetSessionListQuantity()).Returns(this.menuItemsAndQuantities);
            _unitOfWork.Setup(x => x.MenuItemRepository.ReadOne(It.IsAny<int>())).Returns(menuItemsAndQuantities.First(x => x.Item.Id == 1).Item);

            //Act
            OrderDetailsCart tempDetailsCart = this.cartService.PrepareForIndexCart(this.detailsCart);

            //Assert
            Assert.True(tempDetailsCart.ListCart.FirstOrDefault(x => x.Item.Id == 1).Item.Description.Length <= 100);
            Assert.Equal(tempDetailsCart.OrderHeader.OrderTotalOriginal, expectedOrderTotal);
        }

        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[this.random.Next(s.Length)]).ToArray());
        }

        #endregion


        #region Unit test MinusAnItemFromCart method
        
        [Fact]
        public void MinusAnItemFromCart_ItemQuantityEqualOne_ShouldRemoveItem()
        {
            //Arrange
            var requiredItem = this.menuItemsAndQuantities.Find(x => x.Item.Id == 1);
            requiredItem.Quantity = 1;
            _sessionService.Setup(x => x.GetSessionListQuantity()).Returns(this.menuItemsAndQuantities);

            //Act
            List<MenuItemsAndQuantity> resultListItemAndQuantity = this.cartService.MinusAnItemFromCart(requiredItem.Item.Id);

            //Assert
            Assert.Null(resultListItemAndQuantity.FirstOrDefault(x=>x.Item.Id == requiredItem.Item.Id));
        }
        [Fact]
        public void MinusAnItemFromCart_ItemQuantityGreaterThanOne_ShouldRemoveItem()
        {
            //Arrange
            var requiredItem = this.menuItemsAndQuantities.Find(x => x.Item.Id == 1);
            requiredItem.Quantity = 3;
            int expectedQuantity = (requiredItem.Quantity - 1);

            _sessionService.Setup(x => x.GetSessionListQuantity()).Returns(this.menuItemsAndQuantities);

            //Act
            List<MenuItemsAndQuantity> resultListItemAndQuantity = this.cartService.MinusAnItemFromCart(requiredItem.Item.Id);

            //Assert
            Assert.Equal(expectedQuantity, resultListItemAndQuantity.FirstOrDefault(x => x.Item.Id == requiredItem.Item.Id).Quantity);
        }
        #endregion
    }
}
