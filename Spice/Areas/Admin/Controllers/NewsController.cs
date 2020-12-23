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
using Spice.Repository;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class NewsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;
        [BindProperty]
        public NewsViewModel  NewsVM { get; set; }
        public NewsController(IUnitOfWork UnitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = UnitOfWork;
            _hostingEnvironment = hostingEnvironment;
            NewsVM = new NewsViewModel()
            {
                MenuItems = _unitOfWork.MenuItemRepository.ReadAll().ToList(),
                News = new News()
            };
        }


        //GET 
        public IActionResult Index()
        {
            //var news = await _db.News.Include(m => m.MenuItem).Include(n=>n.ApplicationUser).ToListAsync();
            var news = _unitOfWork.NewsRepository.ReadAllNewsIncludeMenuItem();
            return View(news);
        }

        //GET - CREATE
        public IActionResult Create()
        {
            
            if(NewsVM.News.Type != "IntergratedInItem")
            {
                NewsVM.News.MenuItemId = null;
            }
            return View(NewsVM);
        }


        //POST - CREATE
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult CreatePOST(News news)
        {

            if(!ModelState.IsValid)
            {
                return View(NewsVM);
            }

            _unitOfWork.NewsRepository.Create(NewsVM.News);
            _unitOfWork.SaveChanges();

            //Work on the image saving section

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var NewsFromDb = _unitOfWork.NewsRepository.ReadOne(NewsVM.News.Id);

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
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultProductNewsImage);                
                NewsFromDb.ImageHeader = @"\images\" + "DefaultNewsImage" + ".png";
            }
            //_db.News.Add(NewsVM.News);
            _unitOfWork.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        //GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //NewsVM.News = await _db.News.Include(m => m.MenuItem).Include(n=>n.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id);
            NewsVM.News = _unitOfWork.NewsRepository.ReadOneNewsIncludeMenuItem(id);

            if (NewsVM.News == null)
            {
                return NotFound();
            }
            return View(NewsVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPOST(int? id)
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

            var NewsFromDb = _unitOfWork.NewsRepository.ReadOne(NewsVM.News.Id);

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

            _unitOfWork.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //NewsVM.News = await _db.News.Include(m => m.MenuItem).Include(n=>n.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id);
            NewsVM.News = _unitOfWork.NewsRepository.ReadOneNewsIncludeMenuItem(id);

            if (NewsVM.News == null)
            {
                return NotFound();
            }

            return View(NewsVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public  IActionResult DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            News news = _unitOfWork.NewsRepository.ReadOne(id);

            if (news != null)
            {
                var imagePath = Path.Combine(webRootPath, news.ImageHeader.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _unitOfWork.NewsRepository.Delete(news);
                _unitOfWork.SaveChanges();

            }

            return RedirectToAction(nameof(Index));
        }

        //GET - DETAILS
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //NewsVM.News = await _db.News.Include(m => m.MenuItem).Include(n=>n.ApplicationUser).SingleOrDefaultAsync(m => m.Id == id);
            NewsVM.News =  _unitOfWork.NewsRepository.ReadOneNewsIncludeMenuItem(id);

            if (NewsVM.News == null)
            {
                return NotFound();
            }

            return View(NewsVM);
        }

    }
}
