using Microsoft.AspNetCore.Http;
using Moq;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;
using System;
using System.Collections.Generic;
using Xunit;

namespace Spice.UnitTesting
{
    public class DiscountedPriceTest
    {
        MenuItem item1;
        MenuItem item2;
        public DiscountedPriceTest()
        {
            item1 = new MenuItem { Id = 1, Name = "Rolex 1", Description = "Awesome", Size = "41.00 x 41.00mm", Band = "Plastic", Thickness = 9.8, Crystal = "Plexigrass", Image = "\\images\\1.png", Price = 100, IsPublish = true, Quantity = 6, Color = "3", Tag = "2", PublishedDate = DateTime.Now, CategoryId = 2, SubCategoryId = 3 };
            item2 = new MenuItem { Id = 3, Name = "Rolex 3", Description = "Awesome", Size = "41.00 x 41.00mm", Band = "Plastic", Thickness = 9.8, Crystal = "Plexigrass", Image = "\\images\\3.png", Price = 25, IsPublish = true, Quantity = 23, Color = "3", Tag = "2", PublishedDate = DateTime.Now, CategoryId = 2, SubCategoryId = 3 };

        }
        [Fact]
        public void DiscountedPrice_ActivedPercentCoupon_ReturnTrue()
        {
            List<MenuItemsAndQuantity> lstCart = new List<MenuItemsAndQuantity>();
            Coupon coupon = new Coupon { Id = 1, Name = "15OFF", CouponType = "0", Discount = 15, MinimumAmount = 75, IsActive = true };
            lstCart.Add(new MenuItemsAndQuantity()
            {
                Item = item1,                
                Quantity = 3
            }); 
            lstCart.Add(new MenuItemsAndQuantity()
            {
                Item =item2,
                Quantity = 2
            });
            double total = 0;
            foreach (var item in lstCart)
            {
                total += (item.Item.Price * item.Quantity);
            }
            Assert.True(SD.DiscountedPrice(coupon, total) == total*0.85);
        }
        [Fact]
        public void DiscountedPrice_UnactivedCoupon_ReturnTrue()
        {
            List<MenuItemsAndQuantity> lstCart = new List<MenuItemsAndQuantity>();
            Coupon coupon = new Coupon { Id = 1, Name = "15OFF", CouponType = "0", Discount = 15, MinimumAmount = 75, IsActive = false };
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
            double total = 0;
            foreach (var item in lstCart)
            {
                total += (item.Item.Price * item.Quantity);
            }
            Assert.True(SD.DiscountedPrice(coupon, total) == total);
        }
        [Fact]
        public void DiscountedPrice_BigMinimumAmount_ReturnTrue()
        {
            List<MenuItemsAndQuantity> lstCart = new List<MenuItemsAndQuantity>();
            Coupon coupon = new Coupon { Id = 1, Name = "15OFF", CouponType = "0", Discount = 15, MinimumAmount = 500, IsActive = true };
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
            double total = 0;
            foreach (var item in lstCart)
            {
                total += (item.Item.Price * item.Quantity);
            }
            Assert.True(SD.DiscountedPrice(coupon, total) == total);
        }
        [Fact]
        public void DiscountedPrice_UnvalidCoupon_ReturnTrue()
        {
            List<MenuItemsAndQuantity> lstCart = new List<MenuItemsAndQuantity>();
            Coupon coupon = null;
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
            double total = 0;
            foreach (var item in lstCart)
            {
                total += (item.Item.Price * item.Quantity);
            }
            Assert.True(SD.DiscountedPrice(coupon, total) == total);
        }
        [Fact]
        public void DiscountedPrice_ActivedDollarCoupon_ReturnTrue()
        {
            List<MenuItemsAndQuantity> lstCart = new List<MenuItemsAndQuantity>();
            Coupon coupon = new Coupon { Id = 1, Name = "15OFF", CouponType = "1", Discount = 15, MinimumAmount = 75, IsActive = true };
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
            double total = 0;
            foreach (var item in lstCart)
            {
                total += (item.Item.Price * item.Quantity);
            }
            Assert.True(SD.DiscountedPrice(coupon, total) == total-15);
        }

    }
}
