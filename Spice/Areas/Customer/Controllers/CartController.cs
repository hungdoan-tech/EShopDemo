using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spice.Extensions;
using Spice.Models.ViewModels;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;
using System.Collections.Generic;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IFacadeCartService _facadeCartService;

        [BindProperty]
        public OrderDetailsCart detailCart { get; set; }

        public CartController(IFacadeCartService FacadeService)
        {
            _facadeCartService = FacadeService;
        }

        public IActionResult Index()
        {
            detailCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            detailCart.OrderHeader.OrderTotal = 0;
            _facadeCartService.PrepareForIndexCart(detailCart);
            return View(detailCart);
        }

        [Authorize]
        public IActionResult Summary()
        {
            detailCart = new OrderDetailsCart();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            _facadeCartService.CreateOrderHeaderBeforeSumary(detailCart, claim);
            _facadeCartService.CheckCouponBeforeSumary(detailCart);

            return View(detailCart);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            _facadeCartService.SaveObjectsToDB(detailCart,claim);
            _facadeCartService.ApplyCoupon(detailCart);
            _facadeCartService.ChargeMoney(detailCart, stripeToken);
            _facadeCartService.SendCommittedEmail(detailCart, claim);
            _facadeCartService.ClearSession();

            return RedirectToAction("Index", "Home");
        }

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
            ViewBag.Alert = false;
            if(_facadeCartService.CheckCurrentItemQuantity(cartId)==true)
            {
                ViewBag.Alert = true;
            }
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