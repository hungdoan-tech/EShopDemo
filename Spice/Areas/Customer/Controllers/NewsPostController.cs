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

        public NewsPostController(ApplicationDbContext db)
        {
            _db = db;

        }
        //[Route("NewsPost/{Alias}-{Id}")]
        public async Task<IActionResult> NewsPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _db.News.Include(m => m.MenuItem).Where(m=>m.Id == id).SingleOrDefaultAsync();

            return View(news);
        }
        public async Task<IActionResult> Index()
        {

            var news = await _db.News.Include(m=>m.MenuItem).Where(m => m.Type == "1" || m.Type == "2").ToListAsync();

            return View(news);
        }
    }
}
