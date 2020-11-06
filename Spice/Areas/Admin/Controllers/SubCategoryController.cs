using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser +"," + SD.RepositoryManager)]
    public class SubCategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public SubCategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.SubCategoryRepository.ReadAll().ToListAsync());
        }

        [HttpGet]
        public IActionResult Create ()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(SubCategory subCategory)
        {
            if (ModelState.IsValid)
            {
                //if valid
                _unitOfWork.SubCategoryRepository.Create(subCategory);
                _unitOfWork.SaveChanges();

                return RedirectToAction(nameof(Index));

            }
            return View(subCategory);
        }



        //GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subcategory =  _unitOfWork.SubCategoryRepository.ReadOne(id);
            if (subcategory == null)
            {
                return NotFound();
            }
            return View(subcategory);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(SubCategory subCategory)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.SubCategoryRepository.Update(subCategory);
                _unitOfWork.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(subCategory);
        }

        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var subCategory = _unitOfWork.SubCategoryRepository.ReadOne(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            return View(subCategory);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            var subCategory = _unitOfWork.SubCategoryRepository.ReadOne(id);

            if (subCategory == null)
            {
                return View();
            }
            _unitOfWork.SubCategoryRepository.Delete(subCategory);
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

            var subCategory = _unitOfWork.SubCategoryRepository.ReadOne(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return View(subCategory);
        }
    }
}