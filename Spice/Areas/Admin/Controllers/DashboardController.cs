using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spice.Data;
using Spice.Utility;
using System.Linq;
using System.Collections.Generic;
using Spice.Models.Chart;
using Google.DataTable.Net.Wrapper.Extension;
using Google.DataTable.Net.Wrapper;
using Spice.Models;
using System;

namespace Spice.Areas.Admin
{
    [Authorize(Roles = SD.ManagerUser+","+SD.RepositoryManager+","+SD.Shipper)]
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DashboardController(ApplicationDbContext db)
        {
            this._db = db;
        }

        [Route("Admin")]
        [Route("Admin/Dashboard")]
        [Route("Admin/Dashboard/Index")]
        public IActionResult Index()
        {
            OverviewDataDashboad overviewData = new OverviewDataDashboad()
            {
                userCount = _db.ApplicationUser.Count(),
                productCount = _db.MenuItem.Count(),
                likeCount = _db.FavoritedProducts.Count(),
                ratingCount = _db.Ratings.Count()
            };
            return View(overviewData);
        }

        [HttpGet]    
        public IActionResult getBestSellerProducts()
        {
            var listOrderDetails = this._db.OrderDetails.ToList();
            var listSoldProducts = listOrderDetails.Distinct().ToList();
            List<BestSellerProducts> chartData = new List<BestSellerProducts>();            

            foreach (var element in listSoldProducts)
            {
                var itemCount = chartData.Where(a => a.MenuItemId == element.MenuItemId);
                if (itemCount.Count() > 0)
                {
                    var temp = chartData.First(a => a.MenuItemId == element.MenuItemId);
                    temp.Count += element.Count;
                }
                else
                {
                    BestSellerProducts eachProduct = new BestSellerProducts()
                    {
                        MenuItemId = element.MenuItemId,
                        MenuItemName = element.Name,
                        Count = element.Count
                    };
                    chartData.Add(eachProduct);
                }                
            }

            var json = chartData.Take(5).ToGoogleDataTable()
                        .NewColumn(new Column(ColumnType.String, "ProductName"), x => x.MenuItemName)
                        .NewColumn(new Column(ColumnType.Number, "Quantity"), x => x.Count)
                        .Build()
                        .GetJson();

            return Content(json);
        }

        public IActionResult getCategoriesPercent()
        {
            var listMenuItems = this._db.MenuItem.ToList();
            var listCategories = this._db.Category.ToList();

            List<CategoriesPercent> chartData = new List<CategoriesPercent>();

            foreach (var element in listCategories)
            {
                CategoriesPercent temp = new CategoriesPercent()
                {
                    CategoryId = element.Id,
                    CategoryName = element.Name,
                    Count = this.calculateCategoriesPercent(listMenuItems, element.Id)
                };
                chartData.Add(temp);
            }

            var json = chartData.ToGoogleDataTable()
                        .NewColumn(new Column(ColumnType.String, "Category Name"), x => x.CategoryName)
                        .NewColumn(new Column(ColumnType.Number, "Quantity"), x => x.Count)
                        .Build()
                        .GetJson();

            return Content(json);
        }

        public IActionResult getBrandsPercent()
        {
            var listMenuItems = this._db.MenuItem.ToList();
            var listBrands = this._db.SubCategory.ToList();

            List<BrandsPercent> chartData = new List<BrandsPercent>();

            foreach (var element in listBrands)
            {
                BrandsPercent temp = new BrandsPercent()
                {
                    BrandId = element.Id,
                    BrandName = element.Name,
                    Count = this.calculateBrandPercent(listMenuItems, element.Id)
                };
                chartData.Add(temp);
            }

            var json = chartData.ToGoogleDataTable()
                        .NewColumn(new Column(ColumnType.String, "Brand Name"), x => x.BrandName)
                        .NewColumn(new Column(ColumnType.Number, "Quantity"), x => x.Count)
                        .Build()
                        .GetJson();

            return Content(json);
        }

        public IActionResult getProfits()
        {
            DateTime now = DateTime.Now;
            var listOrderHeader = _db.OrderHeader.ToList();
            List<ProfitData> chartData = new List<ProfitData>();
            
            for(int i = 0;i < 5; i++)
            {
                TimeSpan aInterval = new System.TimeSpan(i, 0, 0, 0);
                var considerDate = now.Subtract(aInterval);
                var totalProfitInDay = listOrderHeader.Where(a => a.OrderDate.Day == considerDate.Day).Sum(a => a.OrderTotal);

                ProfitData profitData = new ProfitData()
                {
                    date = considerDate.ToString(),
                    profit = totalProfitInDay
                };
                chartData.Add(profitData);
            }

            var json = chartData.ToGoogleDataTable()
                        .NewColumn(new Column(ColumnType.String, "Date"), x => x.date)
                        .NewColumn(new Column(ColumnType.Number, "Quantity"), x => x.profit)
                        .Build()
                        .GetJson();

            return Content(json);
        }



        public int calculateCategoriesPercent(List<MenuItem> menuItems, int CategoryId)
        {
            int sum = 0;
            foreach (var element in menuItems)
            {
                if(element.CategoryId == CategoryId)
                {
                    sum += 1;
                }
            }
            return sum;
        }

        public int calculateBrandPercent(List<MenuItem> menuItems, int subCategoryId)
        {
            int sum = 0;
            foreach (var element in menuItems)
            {
                if (element.SubCategoryId == subCategoryId)
                {
                    sum += 1;
                }
            }
            return sum;
        }
    }
}