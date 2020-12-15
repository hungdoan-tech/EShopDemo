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
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser + "," +SD.RepositoryManager)]
    public class MenuItemController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }
  
        public MenuItemController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvironment = hostingEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = _unitOfWork.CategoryRepository.ReadAll(),
                SubCategory = _unitOfWork.SubCategoryRepository.ReadAll(),
                MenuItem = new MenuItem()
            };
        }
        public IActionResult Index()
        {
            var menuItems = _unitOfWork.MenuItemRepository.ReadAllIncludeCategoryAndSubCategory();
            return View(menuItems);
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        [HttpPost,ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePOST()
        {
            //MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            //if(!ModelState.IsValid)
            //{
            //    return View(MenuItemVM);
            //}

            MenuItemVM.MenuItem.PublishedDate = DateTime.UtcNow;
            _unitOfWork.MenuItemRepository.Create(MenuItemVM.MenuItem);
            _unitOfWork.SaveChanges();

            //Work on the image saving section

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = _unitOfWork.MenuItemRepository.ReadOne(MenuItemVM.MenuItem.Id);

            if(files.Count>0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads,MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                //no file was uploaded, so use default
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }

            _unitOfWork.SaveChanges();

            return RedirectToAction(nameof(Index));
        }


        //GET - EDIT
        public IActionResult Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = _unitOfWork.MenuItemRepository.ReadOneIncludeCategoryAndSubCategory(id);
            //MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem ==null)
            {
                return NotFound();
            }
            return View(MenuItemVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public IActionResult EditPOST(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            //MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"].ToString());

            if (ModelState.IsValid == false)
            {
                //MenuItemVM.SubCategory = await _db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                return LocalRedirect("/MenuItem/Edit/" + id);
            }

            //Work on the image saving section

            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = _unitOfWork.MenuItemRepository.ReadOne(MenuItemVM.MenuItem.Id);

            if (files.Count > 0)
            {
                //New Image has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original file
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                //we will upload the new file
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_new;
            }

            menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDb.Color = MenuItemVM.MenuItem.Color;
            menuItemFromDb.Tag = MenuItemVM.MenuItem.Tag;
            menuItemFromDb.PublishedDate = DateTime.UtcNow;
            menuItemFromDb.Quantity = MenuItemVM.MenuItem.Quantity;
            menuItemFromDb.IsPublish = MenuItemVM.MenuItem.IsPublish;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;

            _unitOfWork.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        //GET : Details MenuItem
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = _unitOfWork.MenuItemRepository.ReadOneIncludeCategoryAndSubCategory(id);

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //GET : Delete MenuItem
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = _unitOfWork.MenuItemRepository.ReadOneIncludeCategoryAndSubCategory(id);

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }

            return View(MenuItemVM);
        }

        //POST Delete MenuItem
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            MenuItem menuItem = _unitOfWork.MenuItemRepository.ReadOne(id);

            if (menuItem != null)
            {
                var imagePath = Path.Combine(webRootPath, menuItem.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
                _unitOfWork.MenuItemRepository.Delete(menuItem);
                _unitOfWork.SaveChanges();

            }
            return RedirectToAction(nameof(Index));
        }
    }
}