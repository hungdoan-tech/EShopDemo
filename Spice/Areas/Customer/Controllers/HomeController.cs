using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Extensions;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;

        public HomeController(ApplicationDbContext db)
        {
            _db = db;
        }


        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };
            return View(IndexVM);
        }

        public async Task<IActionResult> Search(string name = "")
        {
            //Check name is null replace a empty string.
            if (name == null)
            {
                name = "";
            }

            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };
            IndexVM.MenuItem = IndexVM.MenuItem.Where(m => m.Name.ToLower().Contains(name.ToLower()));
            return View(IndexVM);
        }

        public async Task<IActionResult> Details(int id)
        {
            var menuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();
            MenuItemsAndQuantity menuItemsAndQuantity = new MenuItemsAndQuantity()
            {
                Item = menuItemFromDb,
                Quantity = 1
            };
            return View(menuItemsAndQuantity);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(MenuItemsAndQuantity CartItemObject)
        {
            if(ModelState.IsValid)
            {
                List<MenuItemsAndQuantity> lstShoppingCart = HttpContext.Session.Get<List<MenuItemsAndQuantity>>("ssShoppingCart");
                if (lstShoppingCart == null)
                {
                    lstShoppingCart = new List<MenuItemsAndQuantity>();
                }
                foreach (var a in lstShoppingCart)
                {
                    if(a.Item.Id == CartItemObject.Item.Id)
                    {
                        a.Quantity += CartItemObject.Quantity;
                        HttpContext.Session.Set(SD.ssShoppingCart, lstShoppingCart);
                        return RedirectToAction("Index");
                    }
                }
                lstShoppingCart.Add(CartItemObject);
                HttpContext.Session.Set(SD.ssShoppingCart, lstShoppingCart);               
                return RedirectToAction("Index");
            }
            else
            {
                var menuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == CartItemObject.Item.Id).FirstOrDefaultAsync();

                MenuItemsAndQuantity cartObj = new MenuItemsAndQuantity()
                {
                    Item = menuItemFromDb,
                    Quantity = menuItemFromDb.Id
                };
                return View(cartObj);
            }
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
