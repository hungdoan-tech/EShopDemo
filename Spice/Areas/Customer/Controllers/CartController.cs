using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MailKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Extensions;
using Spice.Models;
using Spice.Models.Builder;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service;
using Spice.Utility;
using Stripe;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        [BindProperty]
        public OrderDetailsCart detailCart { get; set; }

        public CartController(ApplicationDbContext db,IEmailSender emailSender, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _db = db;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
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

                foreach (var eachItem in detailCart.listCart)
                {
                    try
                    {
                        eachItem.Item = await _db.MenuItem.FirstOrDefaultAsync(m => m.Id == eachItem.Item.Id);
                        detailCart.OrderHeader.OrderTotal = detailCart.OrderHeader.OrderTotal + (eachItem.Item.Price * eachItem.Quantity);

                        eachItem.Item.Description = SD.ConvertToRawHtml(eachItem.Item.Description);

                        if (eachItem.Item.Description.Length > 100)
                        {
                            eachItem.Item.Description = eachItem.Item.Description.Substring(0, 99) + "...";
                        }
                    }
                    catch { }
                }
                detailCart.OrderHeader.OrderTotalOriginal = detailCart.OrderHeader.OrderTotal;

                if (HttpContext.Session.GetString(SD.ssCouponCode) != null)
                {
                    detailCart.OrderHeader.CouponCode = HttpContext.Session.GetString(SD.ssCouponCode);
                    var couponFromDb = await _db.Coupon.Where(c => c.Name.ToLower() == detailCart.OrderHeader.CouponCode.ToLower()).FirstOrDefaultAsync();
                    detailCart.OrderHeader.OrderTotal = SD.DiscountedPrice(couponFromDb, detailCart.OrderHeader.OrderTotalOriginal);
                }
            }
            else
            {
                detailCart.listCart = new List<MenuItemsAndQuantity>();
            }
            return View(detailCart);
        }

        [Authorize]
        public IActionResult Summary()
        {
            detailCart = new OrderDetailsCart();
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            CartFacadeService facadeService = new CartFacadeService(_unitOfWork, _httpContextAccessor, _emailSender);
            facadeService.CreateOrderHeaderBeforeSumary(detailCart, claim);
            facadeService.CheckCouponBeforeSumary(detailCart);

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

            CartFacadeService facadeService = new CartFacadeService(_unitOfWork, _httpContextAccessor, _emailSender);
            facadeService.SaveObjectsToDB(detailCart,claim);
            facadeService.ApplyCoupon(detailCart);
            facadeService.ChargeMoney(detailCart, stripeToken);
            facadeService.SendEmailCommitted(detailCart, claim);
            facadeService.ClearSession();

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


        public async Task<IActionResult> Plus(int cartId)
        {
            List<MenuItemsAndQuantity> lstShoppingCart = HttpContext.Session.Get<List<MenuItemsAndQuantity>>(SD.ssShoppingCart);

            var menuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == cartId).FirstOrDefaultAsync();

            int currentQuantity = lstShoppingCart.Find(c => c.Item.Id == cartId).Quantity;

            ViewBag.Alert = false;
            if (currentQuantity >= menuItemFromDb.Quantity)
            {
                ViewBag.Alert = true;
            }
            else
            {
                lstShoppingCart.Find(c => c.Item.Id == cartId).Quantity += 1;
                HttpContext.Session.Set(SD.ssShoppingCart, lstShoppingCart);
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