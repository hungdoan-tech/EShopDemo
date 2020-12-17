using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spice.Data;
using Spice.Utility;
using System.Linq;
using System.Collections.Generic;
using Spice.Models.Chart;
using Google.DataTable.Net.Wrapper.Extension;
using Google.DataTable.Net.Wrapper;

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
            return View();
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
    }
}