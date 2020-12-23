using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spice.Extensions;
using Spice.Models.ViewModels;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;


namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IFacadeCartService _facadeCartService;

        [BindProperty]
        public OrderDetailsCart DetailCart { get; set; }

        public CartController(IFacadeCartService FacadeService)
        {
            _facadeCartService = FacadeService;
        }

        public IActionResult Index()
        {
            DetailCart = new OrderDetailsCart()
            {
                OrderHeader = new Models.OrderHeader()
            };

            DetailCart.OrderHeader.OrderTotal = 0;
            _facadeCartService.PrepareForIndexCart(DetailCart);
            return View(DetailCart);
        }

        [Authorize]
        public IActionResult Summary()
        {
            DetailCart = new OrderDetailsCart();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            _facadeCartService.CreateOrderHeaderBeforeSumary(DetailCart, claim);
            _facadeCartService.CheckCouponBeforeSumary(DetailCart);

            return View(DetailCart);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            _facadeCartService.SaveObjectsToDB(DetailCart,claim);
            _facadeCartService.ApplyCoupon(DetailCart);
            _facadeCartService.ChargeMoney(DetailCart, stripeToken);
            _facadeCartService.SendCommittedEmail(DetailCart, claim);
            _facadeCartService.ClearSession();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AddCoupon()
        {
            if (DetailCart.OrderHeader.CouponCode == null)
            {
                DetailCart.OrderHeader.CouponCode = "";
            }
            HttpContext.Session.SetString(SD.ssCouponCode, DetailCart.OrderHeader.CouponCode);
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