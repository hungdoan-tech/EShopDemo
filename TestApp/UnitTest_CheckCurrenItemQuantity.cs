using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Spice.Extensions;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service;
using Spice.Utility;
using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Core.Persistence.Repositories;
using Xunit;

namespace TestApp
{
    [TestClass]
    public class UnitTest_CheckCurrenItemQuantity
    {
        /*[TestMethod]
        public void CheckQuantity_ValidQuantity_ReturnTrue()
        {
            var item1 = new MenuItem { Id = 1, Name = "Rolex 1", Description = "Awesome", Size = "41.00 x 41.00mm", Band = "Plastic", Thickness = 9.8, Crystal = "Plexigrass", Image = "\\images\\1.png", Price = 100, IsPublish = true, Quantity = 6, Color = "3", Tag = "2", PublishedDate = DateTime.Now, CategoryId = 2, SubCategoryId = 3 };
            var item2 = new MenuItem { Id = 3, Name = "Rolex 3", Description = "Awesome", Size = "41.00 x 41.00mm", Band = "Plastic", Thickness = 9.8, Crystal = "Plexigrass", Image = "\\images\\3.png", Price = 25, IsPublish = true, Quantity = 23, Color = "3", Tag = "2", PublishedDate = DateTime.Now, CategoryId = 2, SubCategoryId = 3 };
            List<MenuItemsAndQuantity> lstCart = new List<MenuItemsAndQuantity>();
            lstCart.Add(new MenuItemsAndQuantity()
            {
                Item = item1,
                Quantity = 3
            });
            lstCart.Add(new MenuItemsAndQuantity()
            {
                Item = item2,
                Quantity = 2
            });
            var itemRepositoryMock = new Mock<MenuItemRepository>();
            itemRepositoryMock.Setup(u => u.ReadAll()).Returns();
            var apply = new CartService(null, itemRepositoryMock.Object);
            var result = apply.CheckCurrentItemQuantity(1);

            Assert.IsTrue(result == false);
        }*//*
        [Fact]
        public async Task test_GetBookByBookId()
        {
            //Arrange

            //Mock IHttpContextAccessor
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            var fakeTenantId = "1";
            context.Request.Headers["Id"] = fakeTenantId;
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(context);
            //Mock HeaderConfiguration
            var mockHeaderConfiguration = new Mock<IUnitOfWork>();

            var item = new CartService(mockHeaderConfiguration.Object, mockHttpContextAccessor.Object);

            var itemId = 1;

            //Act
            var result = item.CheckCurrentItemQuantity(itemId);

            //Assert
            result.Should().NotBeNull().And.
                BeOfType<List<BookModel>>();
        }*/
    }
}
