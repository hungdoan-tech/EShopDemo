using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductsListController : Controller
    {
        private readonly ApplicationDbContext _db;
        private int PageSize = 9;

        public ProductsListViewModel ProductsListVM { get; set; }

        public ProductsListController(ApplicationDbContext db)
        {
            _db = db;

            ProductsListVM = new ProductsListViewModel()
            {
                Products = _db.MenuItem.ToList(),
                Categories = _db.Category.ToList(),
                SubCategories = db.SubCategory.ToList()
            };
        }

        public async Task<IActionResult> Index(int productPage = 1, string searchName = null, string groupProductsSelected = "Default"
            , string orderBy = "descDate")
        {

            ProductsListVM.Products = await _db.MenuItem.Include(m => m.Category).ToListAsync();

            StringBuilder param = new StringBuilder();
            param.Append("/Customer/ProductsList?productPage=:");

            if (searchName != null)
            {
                param.Append("&searchName=");
                param.Append(searchName);
            }

            if (searchName != null)
            {
                ProductsListVM.Products = ProductsListVM.Products.Where(p => p.Name.ToLower().Trim().Contains(searchName.ToLower().Trim())).ToList();
            }

            var count = ProductsListVM.Products.Count();

            //ProductsListVM.Products = ProductsListVM.Products.OrderBy(p => p.Name)
            //    .Skip((productPage - 1) * PageSize).Take(PageSize).ToList();

            if (!groupProductsSelected.Equals("Default"))
            {
                switch (orderBy)
                {
                    case "ascName":
                        ProductsListVM.Products = ProductsListVM.Products.OrderBy(p => p.Name)
                        .Skip((productPage - 1) * PageSize).Take(PageSize).Where(m => m.Category.Name.Equals(groupProductsSelected)).ToList();
                        break;
                    case "descName":
                        ProductsListVM.Products = ProductsListVM.Products.OrderByDescending(p => p.Name)
                       .Skip((productPage - 1) * PageSize).Take(PageSize).Where(m => m.Category.Name.Equals(groupProductsSelected)).ToList();
                        break;
                    case "ascPrice":
                        ProductsListVM.Products = ProductsListVM.Products.OrderBy(p => p.Price)
                        .Skip((productPage - 1) * PageSize).Take(PageSize).Where(m => m.Category.Name.Equals(groupProductsSelected)).ToList();
                        break;
                    case "descPrice":
                        ProductsListVM.Products = ProductsListVM.Products.OrderByDescending(p => p.Price)
                       .Skip((productPage - 1) * PageSize).Take(PageSize).Where(m => m.Category.Name.Equals(groupProductsSelected)).ToList();
                        break;
                    case "ascDate":
                        ProductsListVM.Products = ProductsListVM.Products.OrderBy(p => p.PublishedDate)
                        .Skip((productPage - 1) * PageSize).Take(PageSize).Where(m => m.Category.Name.Equals(groupProductsSelected)).ToList();
                        break;
                    case "descDate":
                        ProductsListVM.Products = ProductsListVM.Products.OrderByDescending(p => p.PublishedDate)
                        .Skip((productPage - 1) * PageSize).Take(PageSize).Where(m => m.Category.Name.Equals(groupProductsSelected)).ToList();
                        break;
                    case "mostPopuler":
                        ProductsListVM.Products = ProductsListVM.Products.OrderByDescending(p => p.PublishedDate)
                        .Skip((productPage - 1) * PageSize).Take(PageSize).Where(m => m.Tag == MenuItem.ETag.Popular.ToString()).ToList();
                        break;
                }
            }
            else
            {
                switch (orderBy)
                {
                    case "ascName":
                        ProductsListVM.Products = ProductsListVM.Products.OrderBy(p => p.Name)
                        .Skip((productPage - 1) * PageSize).Take(PageSize).ToList();
                        break;
                    case "descName":
                        ProductsListVM.Products = ProductsListVM.Products.OrderByDescending(p => p.Name)
                       .Skip((productPage - 1) * PageSize).Take(PageSize).ToList();
                        break;
                    case "ascPrice":
                        ProductsListVM.Products = ProductsListVM.Products.OrderBy(p => p.Price)
                        .Skip((productPage - 1) * PageSize).Take(PageSize).ToList();
                        break;
                    case "descPrice":
                        ProductsListVM.Products = ProductsListVM.Products.OrderByDescending(p => p.Price)
                       .Skip((productPage - 1) * PageSize).Take(PageSize).ToList();
                        break;
                    case "ascDate":
                        ProductsListVM.Products = ProductsListVM.Products.OrderBy(p => p.PublishedDate)
                        .Skip((productPage - 1) * PageSize).Take(PageSize).ToList();
                        break;
                    case "descDate":
                        ProductsListVM.Products = ProductsListVM.Products.OrderByDescending(p => p.PublishedDate)
                        .Skip((productPage - 1) * PageSize).Take(PageSize).ToList();
                        break;
                    case "mostPopuler":
                        ProductsListVM.Products = ProductsListVM.Products.OrderByDescending(p => p.PublishedDate)
                        .Skip((productPage - 1) * PageSize).Take(PageSize).Where(m => m.Tag == MenuItem.ETag.Popular.ToString()).ToList();
                        break;
                }
            }



            ProductsListVM.PagingInfo = new PagingInfo()
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                urlParam = param.ToString()
            };

            ViewBag.orderBy = orderBy;
            //ViewBag.groupProductsSelected = groupProductsSelected;

            return View(ProductsListVM);
        }
    }
}