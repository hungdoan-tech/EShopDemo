using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Extensions;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service;
using Spice.Service.ServiceInterfaces;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spice.Controllers
{
    [Area("Customer")]   
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;

        private readonly IHomeService _homeService;
        private readonly int PageSize = 3;

        public HomeController(ApplicationDbContext db, IUnitOfWork unitOfWork, IUserService userService, HomeService homeService)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _userService = userService;
            _homeService = homeService;
        }


        public IActionResult Index()
        {
            IndexHomeVM IndexVM = this._homeService.PrepareForHomeIndex();            
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

                MenuItem = await _db.MenuItem.Where(a => a.IsPublish != false)
                                             .Include(m => m.Category)
                                             .Include(m => m.SubCategory)
                                             .ToListAsync(),
                Category = await _db.Category.ToListAsync(),
                Coupon = await _db.Coupon.Where(c => c.IsActive == true)
                                         .ToListAsync()
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
        public async Task<IActionResult> Details(int id, int starNumber = 0, int ratingPage = 1)
        {
            var userId = _userService.GetUserId();
            var menuItemFromDb = await _db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();

            var listStar = _db.Ratings.Where(a => a.MenuItemId == id).Include(a => a.ApplicationUser).OrderByDescending(p=>p.Id).ToList();


            StringBuilder param = new StringBuilder();
            var listRatingByStarNumber = listStar.ToList();
            if (starNumber == 0)
            {
                listRatingByStarNumber = listStar.ToList();
            }
            else if(starNumber > 0)
            {
                listRatingByStarNumber = listRatingByStarNumber.Where(a => a.RatingStar == starNumber).ToList();

            }    
            
            param.Append("/Customer/Home/Details/" + id + "?starNumber="+starNumber + "&ratingPage=:");

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
                productStar.averageStar = Math.Round(listStar.Average(a => a.RatingStar),1);
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
            ViewBag.itemColor = Enum.GetName(typeof(MenuItem.EColor), Convert.ToInt32(menuItemFromDb.Color));

            var highLimitPrice = menuItemFromDb.Price + (menuItemFromDb.Price * 25 / 100);
            var lowLimitPrice = menuItemFromDb.Price - (menuItemFromDb.Price * 25 / 100);

            var similarPriceProducts = _db.MenuItem.Take(4);

            MenuItemsAndQuantity menuItemsAndQuantity = new MenuItemsAndQuantity()
            {
                Item = menuItemFromDb,
                Quantity = 1,
                News = await _db.News.Where(n => n.MenuItemId == id).FirstOrDefaultAsync(),
                SimilarProducts = similarPriceProducts.ToList(),
                ExistedRatings = listRatingByStarNumber.Skip((ratingPage - 1)*PageSize).Take(PageSize).ToList(),
                ProductStar = productStar,
                FavoritedProduct = favoritedProduct
            };
            var count = listRatingByStarNumber.Count();
            menuItemsAndQuantity.PagingInfo = new PagingInfo()
            {
                CurrentPage = ratingPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                urlParam = param.ToString()
            };
            ViewBag.starNumber = starNumber;
            ViewBag.pageSize = this.PageSize;
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
            if (!ModelState.IsValid)
            {
                List<MenuItemsAndQuantity> lstShoppingCart = HttpContext.Session.Get<List<MenuItemsAndQuantity>>("ssShoppingCart");
                if (lstShoppingCart == null)
                {
                    lstShoppingCart = new List<MenuItemsAndQuantity>();
                }
                foreach (var a in lstShoppingCart)
                {
                    if (a.Item.Id == CartItemObject.Item.Id)
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

        [HttpGet]
        [Authorize]
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

        [HttpGet]
        [Authorize]
        public IActionResult CreateRating(MenuItemsAndQuantity temp)
        {            
            var userId = _userService.GetUserId();

            Rating rating = new Rating()
            {
                UserId = userId,
                PublishedDate = temp.CustomerRating.PublishedDate,
                RatingStar = temp.CustomerRating.RatingStar,
                Comment = temp.CustomerRating.Comment,
                MenuItemId = temp.CustomerRating.MenuItemId
            };
                
            _unitOfWork.RatingRepository.Create(rating);
            _unitOfWork.SaveChanges();
            return LocalRedirect("/Customer/Home/Details/" + temp.CustomerRating.MenuItemId);
        }

        [HttpGet]
        [Authorize]
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

        [HttpGet]
        public IActionResult AfterRegistering()
        {
            return View();
        }
    }
}
