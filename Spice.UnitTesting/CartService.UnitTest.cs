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
        MenuItem item1;
        MenuItem item2;

        public CartServiceTest()
        {
            this._cartService = new CartService(_unitOfWork.Object, _httpContextAccessor.Object, _sessionService.Object);
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
        }

        [Fact]
        public void CheckCurrentItemQuantity_ValidQuantity_ReturnFalse()
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
            bool result = _cartService.CheckCurrentItemQuantity(1);

            //Assert
            Assert.False(result);
        }
        [Fact]
        public void CheckCurrentItemQuantity_InvalidQuantity_ReturnTrue()
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
            bool result = _cartService.CheckCurrentItemQuantity(1);

            //Assert
            Assert.True(result);
        }
    }
}
