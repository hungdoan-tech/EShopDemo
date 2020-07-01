using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;
        [BindProperty]
        public NewsViewModel  NewsVM { get; set; }
        public NewsController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            NewsVM = new NewsViewModel()
            {
                MenuItems = _db.MenuItem,
                ApplicationUsers = _db.ApplicationUser,
                News = new Models.News()
            };
        }


        //GET 
        public async Task<IActionResult> Index()
        {
            //var news = await _db.News.Include(m => m.MenuItem).Include(n=>n.ApplicationUser).ToListAsync();
            var news = await _db.News.Include(m => m.MenuItem).ToListAsync();
            return View(news);
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View(NewsVM);
        }


        //POST - CREATE
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST(News news)
        {

            if(!ModelState.IsValid)
            {
                return View(NewsVM);
            }

            _db.News.Add(NewsVM.News);
            await _db.SaveChangesAsync();

            //Work on the image saving section

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var NewsFromDb = await _db.News.FindAsync(NewsVM.News.Id);

            if(files.Count>0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads, "News" + NewsVM.News.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                NewsFromDb.ImageHeader = @"\images\"  + "News" + NewsVM.News.Id + extension;
            }
            else
            {
                //no file was uploaded, so use default
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" +  "DefaultNewsImage" + ".png");
                NewsFromDb.ImageHeader = @"\images\" + "DefaultNewsImage" + ".png";
            }
            //_db.News.Add(NewsVM.News);
            //await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //GET - EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //NewsVM.News = await _db.News.Include(m => m.MenuItem).Include(n=>n.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id);
            NewsVM.News = await _db.News.Include(m => m.MenuItem).SingleOrDefaultAsync(m => m.Id == id);

            if (NewsVM.News == null)
            {
                return NotFound();
            }
            return View(NewsVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPOST(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                //NewsVM.NewsCategory = await _db.NewsCategories.Where(s => s.Id == NewsVM.News.NewsCategoryId).ToListAsync();
                return View(NewsVM);
            }

            //Work on the image saving section


            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var NewsFromDb = await _db.News.FindAsync(NewsVM.News.Id);

            if (files.Count > 0)
            {
                //New Image has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original file
                var imagePath = Path.Combine(webRootPath, NewsFromDb.ImageHeader.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                //we will upload the new file
                using (var filesStream = new FileStream(Path.Combine(uploads, "News" + NewsVM.News.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                NewsFromDb.ImageHeader = @"\images\" + "News" + NewsVM.News.Id + extension_new;
            }

            NewsFromDb.Alias = NewsVM.News.Alias;
            //NewsFromDb.ApplicationUserId = NewsVM.News.ApplicationUserId;
            NewsFromDb.PublishedDate = NewsVM.News.PublishedDate;
            NewsFromDb.Content = NewsVM.News.Content;
            NewsFromDb.Header = NewsVM.News.Header;
            NewsFromDb.MenuItemId = NewsVM.News.MenuItemId;
            NewsFromDb.Type = NewsVM.News.Type;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //GET - DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //NewsVM.News = await _db.News.Include(m => m.MenuItem).Include(n=>n.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id);
            NewsVM.News = await _db.News.Include(m => m.MenuItem).SingleOrDefaultAsync(m => m.Id == id);

            if (NewsVM.News == null)
            {
                return NotFound();
            }

            return View(NewsVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            News news = await _db.News.FindAsync(id);

            if (news != null)
            {
                var imagePath = Path.Combine(webRootPath, news.ImageHeader.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _db.News.Remove(news);
                await _db.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));
        }

        //GET - DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //NewsVM.News = await _db.News.Include(m => m.MenuItem).Include(n=>n.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id);
            NewsVM.News = await _db.News.Include(m => m.MenuItem).SingleOrDefaultAsync(m => m.Id == id);

            if (NewsVM.News == null)
            {
                return NotFound();
            }

            return View(NewsVM);
        }

    }
}
