﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Extensions;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;
using Stripe;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public OrderDetailsCart detailCart { get; set; }

        public CartController(ApplicationDbContext db,IEmailSender emailSender)
        {
            _db = db;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {

            detailCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            detailCart.OrderHeader.OrderTotal = 0;

            var cart = HttpContext.Session.Get<List<MenuItemsAndQuantity>>(SD.ssShoppingCart);

            if (cart != null)
            {
                detailCart.listCart = cart.ToList();
            }

            foreach (var eachItem in detailCart.listCart)
            {
                eachItem.Item = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == eachItem.Item.Id);
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + (eachItem.Item.Price * eachItem.Quantity);
                eachItem.Item.Description = SD.ConvertToRawHtml(eachItem.Item.Description);
                
                if (eachItem.Item.Description.Length > 100)
                {
                    eachItem.Item.Description = eachItem.Item.Description.Substring(0, 99) + "...";
                }
            }
            detailCart.OrderHeader.OrderTotalOriginal = detailCart.OrderHeader.OrderTotal;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == detailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.OrderHeader.OrderTotalOriginal);
            }
            return View(detailCart);
        }

        [Authorize]
        public async Task<IActionResult> Summary()
        {
            detailCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            detailCart.OrderHeader.OrderTotal = 0;

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ApplicationUser applicationUser = await _db.ApplicationUser.Where(c => c.Id == claim.Value).FirstOrDefaultAsync();

            var cart = HttpContext.Session.Get<List<MenuItemsAndQuantity>>(SD.ssShoppingCart);

            if (cart != null)
            {
                detailCart.listCart = cart.ToList();
            }


            foreach (var eachItem in detailCart.listCart)
            {
                eachItem.Item = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == eachItem.Item.Id);
                detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + (eachItem.Item.Price * eachItem.Quantity);
            }
            detailCart.OrderHeader.OrderTotalOriginal = detailCart.OrderHeader.OrderTotal;
            detailCart.OrderHeader.PickupName = applicationUser.Name;
            detailCart.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            detailCart.OrderHeader.UserId = claim.Value;

            if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
            {
                detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == detailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                detailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.OrderHeader.OrderTotalOriginal);
            }
            return View(detailCart);
        }

        //[Authorize]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ActionName("Summary")]
        //public async Task<IActionResult> SummaryPost(string stripeToken)
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


        //    detailCart.listCart = await _db.ShoppingCart.Where(c => c.ApplicationUserId == claim.Value).ToListAsync();

        //    detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
        //    detailCart.OrderHeader.OrderDate = DateTime.Now;
        //    detailCart.OrderHeader.UserId = claim.Value;
        //    detailCart.OrderHeader.Status = SD.PaymentStatusPending;
        //    detailCart.OrderHeader.PickUpTime = Convert.ToDateTime(detailCart.OrderHeader.PickUpDate.ToShortDateString() + " " + detailCart.OrderHeader.PickUpTime.ToShortTimeString());

        //    List<OrderDetails> orderDetailsList = new List<OrderDetails>();
        //    _db.OrderHeader.Add(detailCart.OrderHeader);
        //    await _db.SaveChangesAsync();

        //    detailCart.OrderHeader.OrderTotalOriginal = 0;


        //    foreach (var item in detailCart.listCart)
        //    {
        //        item.MenuItem = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == item.MenuItemId);
        //        OrderDetails orderDetails = new OrderDetails
        //        {
        //            MenuItemId = item.MenuItemId,
        //            OrderId = detailCart.OrderHeader.Id,
        //            Description = item.MenuItem.Description,
        //            Name = item.MenuItem.Name,
        //            Price = item.MenuItem.Price,
        //            Count = item.Count
        //        };
        //        detailCart.OrderHeader.OrderTotalOriginal += orderDetails.Count * orderDetails.Price;
        //        _db.OrderDetails.Add(orderDetails);

        //    }

        //    if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
        //    {
        //        detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
        //        var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == detailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
        //        detailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.OrderHeader.OrderTotalOriginal);
        //    }
        //    else
        //    {
        //        detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotalOriginal;
        //    }
        //    detailCart.OrderHeader.CouponCodeDiscount = detailCart.OrderHeader.OrderTotalOriginal - detailCart.OrderHeader.OrderTotal;

        //    _db.ShoppingCart.RemoveRange(detailCart.listCart);
        //    HttpContext.Session.SetInt32(SD.ssShoppingCartCount, 0);
        //    await _db.SaveChangesAsync();

        //    var options = new ChargeCreateOptions
        //    {
        //        Amount = Convert.ToInt32(detailCart.OrderHeader.OrderTotal * 100),
        //        Currency = "usd",
        //        Description = "Order ID : " + detailCart.OrderHeader.Id,
        //        Source = stripeToken

        //    };
        //    var service = new ChargeService();
        //    Charge charge = service.Create(options);

        //    if (charge.BalanceTransactionId == null)
        //    {
        //        detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
        //    }
        //    else
        //    {
        //        detailCart.OrderHeader.TransactionId = charge.BalanceTransactionId;
        //    }

        //    if (charge.Status.ToLower() == "succeeded")
        //    {
        //        detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusApproved;
        //        detailCart.OrderHeader.Status = SD.StatusSubmitted;
        //    }
        //    else
        //    {
        //        detailCart.OrderHeader.PaymentStatus = SD.PaymentStatusRejected;
        //    }

        //    await _db.SaveChangesAsync();
        //    return RedirectToAction("Index", "Home");
        //}


        public IActionResult AddCoupon()
        {
            if (detailCart.OrderHeader.CouponCode == null)
            {
                detailCart.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, detailCart.OrderHeader.CouponCode);

            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveCoupon()
        {
            HttpContext.Session.SetString(SD.ssCouponCode, string.Empty);
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Plus(int cartId)
        {
            List<MenuItemsAndQuantity> lstShoppingCart = HttpContext.Session.Get<List<MenuItemsAndQuantity>>(SD.ssShoppingCart);
            lstShoppingCart.Find(c => c.Item.Id == cartId).Quantity += 1;
            HttpContext.Session.Set(SD.ssShoppingCart, lstShoppingCart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Minus(int cartId)
        {
            List<MenuItemsAndQuantity> lstShoppingCart = HttpContext.Session.Get<List<MenuItemsAndQuantity>>(SD.ssShoppingCart);
            var cartItem = lstShoppingCart.Find(c => c.Item.Id == cartId);
            if (cartItem.Quantity == 1)
            {
                lstShoppingCart.Remove(cartItem);
            }
            else
            {
                lstShoppingCart.Find(c => c.Item.Id == cartId).Quantity -= 1;
            }
            HttpContext.Session.Set(SD.ssShoppingCart, lstShoppingCart);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            List<MenuItemsAndQuantity> lstShoppingCart = HttpContext.Session.Get<List<MenuItemsAndQuantity>>(SD.ssShoppingCart);
            var cartItem = lstShoppingCart.Find(c => c.Item.Id == cartId);
            lstShoppingCart.Remove(cartItem);
            HttpContext.Session.Set(SD.ssShoppingCart, lstShoppingCart);
            return RedirectToAction(nameof(Index));
        }
    }
}