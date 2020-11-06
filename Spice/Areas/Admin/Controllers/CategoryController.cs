using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Repository;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser + "," + SD.RepositoryManager)]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //GET 
        public IActionResult Index()
        {
            return View(_unitOfWork.CategoryRepository.ReadAll().ToList());
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }


        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if(ModelState.IsValid)
            {
                //if valid
                _unitOfWork.CategoryRepository.Create(category);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }


        //GET - EDIT
        public IActionResult Edit(int? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var category = _unitOfWork.CategoryRepository.ReadOne(id);
            if(category==null)
            {
                return NotFound();
            }
            return View(category);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.CategoryRepository.Update(category);
                _unitOfWork.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var category = _unitOfWork.CategoryRepository.ReadOne(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            var category = _unitOfWork.CategoryRepository.ReadOne(id);
            if(category ==null)
            {
                return View();
            }
            _unitOfWork.CategoryRepository.Delete(category);
            _unitOfWork.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        //GET - DETAILS
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = _unitOfWork.CategoryRepository.ReadOne(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

    }
}