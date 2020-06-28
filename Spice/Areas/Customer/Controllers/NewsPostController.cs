using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class NewsPostController : Controller
    {
        private readonly ApplicationDbContext _db;
       
        [BindProperty]
        public NewsViewModel NewsVM { get; set; }

        public NewsPostController(ApplicationDbContext db )
        {
            _db = db;
            NewsVM = new NewsViewModel()
            {
                NewsCategory = _db.NewsCategories,
                News = new Models.News()
            };

        }
        [Route("NewsPost/{Alias}-{Id}")]
        public async Task<IActionResult> SpecificNews(int ?id)
        {
            if (id == null)
            {
                return NotFound();
            }

            NewsVM.News = await _db.News.Include(m => m.NewsCategory).Where(m => m.Id == id).SingleOrDefaultAsync();

            if (NewsVM.News == null)
            {
                return NotFound();
            }

            return View(NewsVM);
        }
    }
}
