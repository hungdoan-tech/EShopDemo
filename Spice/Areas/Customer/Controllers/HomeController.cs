using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Extensions;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service;
using Spice.Utility;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        //Once a page has limit 3 products.
        private int PageSize = 3;
        private readonly IUserService _userService;
        public HomeController(ApplicationDbContext db, IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _userService = userService;
        }


        public async Task<IActionResult> Index(int productPage = 1)
        {
            IndexHomeVM IndexVM = new IndexHomeVM()
            {
                ListPopularMenuItem = await _db.MenuItem.Where(a => a.Tag == "2" && a.IsPublish == true).OrderByDescending(a => a.Id).Take(6).Include(a => a.Category).Include(a => a.SubCategory).ToListAsync(),
                ListNewMenuItem = await _db.MenuItem.Where(a => a.Tag == "1" && a.IsPublish == true).OrderByDescending(a => a.Id).Take(6).Include(a => a.Category).Include(a => a.SubCategory).ToListAsync(),
                ListBestSellerMenuItem = await _db.MenuItem.Where(a => a.Tag == "0" && a.IsPublish == true).OrderByDescending(a => a.Id).Take(2).Include(a => a.Category).Include(a => a.SubCategory).ToListAsync(),
                ListNews = await _db.News.Where(a => a.Type != "0").OrderByDescending(a => a.PublishedDate).Take(2).ToListAsync()
            };
            return View(IndexVM);
        }

        public async Task<IActionResult> Search(string name = "", int productPage = 1)
        {
            //Check name is null replace a empty string.
            if (name == null)
            {
                name = "";
            }

            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = await _db.MenuItem.Where(a => a.IsPublish != false).Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };

            //Pagination: - Url determine current pages.
            StringBuilder param = new StringBuilder();
            param.Append("/Customer/Home/Search?productPage=:");

            param.Append("&name=");
            if (name != null)
            {
                param.Append(name);
            }

            IndexVM.MenuItem = IndexVM.MenuItem.Where(m => m.Name.ToLower().Contains(name.ToLower()));

            //Count a quantity in MenuItem.
            var count = IndexVM.MenuItem.Count();

            IndexVM.MenuItem = IndexVM.MenuItem.OrderBy(p => p.Price)
               .Skip((productPage - 1) * PageSize).Take(PageSize).ToList();


            IndexVM.PagingInfo = new PagingInfo()
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                urlParam = param.ToString()
            };

            return View(IndexVM);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userService.GetUserId();
            var menuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();
            var listStar = _db.Ratings.Where(a => a.MenuItemId == id).Include(a => a.ApplicationUser).ToList();
           
            FavoritedProduct favoritedProduct = new FavoritedProduct();
            if (userId != null)
            {
                var favor = _db.FavoritedProducts.FirstOrDefault(a => a.ItemId == id && a.UserId == userId);
                if (favor != null)
                {
                    favoritedProduct.ItemId = favor.ItemId;
                    favoritedProduct.UserId = favor.UserId;
                }
            }
            ProductStar productStar = new ProductStar();
            if (listStar.Count > 0)
            {
                productStar.totalOneStar = listStar.Where(a => a.RatingStar == 1).Count();
                productStar.totalTwoStar = listStar.Where(a => a.RatingStar == 2).Count();
                productStar.totalThreeStar = listStar.Where(a => a.RatingStar == 3).Count();
                productStar.totalFourStar = listStar.Where(a => a.RatingStar == 4).Count();
                productStar.totalFiveStar = listStar.Where(a => a.RatingStar == 5).Count();
                productStar.averageStar = listStar.Average(a => a.RatingStar);
            }
            else {
                productStar.totalOneStar = 0;
                productStar.totalTwoStar = 0;
                productStar.totalThreeStar = 0;
                productStar.totalFourStar = 0;
                productStar.totalFiveStar = 0;
                productStar.averageStar = 0;
            }
            //Convert Enum Color -> String
            /*  ViewBag.itemColor = Enum.GetName(typeof(MenuItem.EColor), Convert.ToInt32(menuItemFromDb.Color));*/

            MenuItemsAndQuantity menuItemsAndQuantity = new MenuItemsAndQuantity()
            {
                Item = menuItemFromDb,
                Quantity = 1,
                News = await _db.News.Where(n => n.MenuItemId == id).FirstOrDefaultAsync(),
                ExistedRatings = listStar.Take(3).ToList(),
                ProductStar = productStar,
                FavoritedProduct = favoritedProduct
            };

            return View(menuItemsAndQuantity);

        }

        public async Task<IActionResult> CheckQuantity(int id)
        {
            var menuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();

            return Ok(menuItemFromDb.Quantity);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(MenuItemsAndQuantity CartItemObject)
        {
            if(!ModelState.IsValid)
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

        [Route("/Home/FavoriteProductConfirm/{id}")]
        public IActionResult FavoriteProductConfirm(int id)
        {
            string userId = _userService.GetUserId();
            int productId = id;
            var temp = _db.FavoritedProducts.FirstOrDefault(a => a.ItemId == productId && a.UserId == userId);
            if(temp == null)
            {
                FavoritedProduct favoritedProduct = new FavoritedProduct()
                {
                    ItemId = productId,
                    UserId = userId
                };
                _db.FavoritedProducts.Add(favoritedProduct);
            }
            else
            {
                _db.Remove(temp);
            }
            _db.SaveChanges();
            return LocalRedirect("/Customer/Home/Details/" + id);
        }

        [Authorize]
        public IActionResult CreateRating(MenuItemsAndQuantity temp)
        {            
            var userId = _userService.GetUserId();
            Rating rating = new Rating();
            rating.UserId = userId;
            rating.PublishedDate = temp.CustomerRating.PublishedDate;
            rating.RatingStar = temp.CustomerRating.RatingStar;
            rating.Comment = temp.CustomerRating.Comment;
            rating.MenuItemId = temp.CustomerRating.MenuItemId;
                
            _unitOfWork.RatingRepository.Create(rating);
            _unitOfWork.SaveChanges();
            return LocalRedirect("/Customer/Home/Details/" + temp.CustomerRating.MenuItemId);
        }
        [HttpGet]
        public IActionResult FavoriteProductIndex()
        {
            var userId = _userService.GetUserId();
            var listFavor = _db.FavoritedProducts.Include(a => a.MenuItem).Where(a => a.UserId == userId).ToList();

            return View(listFavor);
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
