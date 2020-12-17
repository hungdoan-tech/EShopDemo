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
            List<int> listSoldProducts = listOrderDetails.Distinct().Select(a=>a.Id).ToList();
            List<SoldProducts> soldProducts = new List<SoldProducts>();            

            foreach (var element in listSoldProducts)
            {
                SoldProducts eachProduct = new SoldProducts()
                {
                    ProductId = element,
                    Count = listOrderDetails.Where(a => a.Id == element).Select(a => a.Count).Sum()
                };   
                soldProducts.Add(eachProduct);
            }

            var json = soldProducts.ToGoogleDataTable()
                        .NewColumn(new Column(ColumnType.String, "ProductID"), x => x.ProductId)
                        .NewColumn(new Column(ColumnType.Number, "Quantity"), x => x.Count)
                        .Build()
                        .GetJson();

            return Content(json);
        }        
    }
}