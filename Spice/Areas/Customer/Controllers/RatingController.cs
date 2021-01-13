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

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class RatingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public RatingController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        //GET 
        public IActionResult Index()
        {
            return View();
        }

        //GET - CREATE
        public IActionResult Create()
        {
            return View();
        }


        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Rating rating)
        {
            if (ModelState.IsValid)
            {
                //if valid
                _unitOfWork.RatingRepository.Create(rating);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(rating);
        }


        //GET - EDIT
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var rating = _unitOfWork.RatingRepository.ReadOne(id);
            if (rating == null)
            {
                return NotFound();
            }
            return View(rating);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Rating rating)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.RatingRepository.Update(rating);
                _unitOfWork.SaveChanges();

                return RedirectToAction(nameof(Index));
            }
            return View(rating);
        }

        //GET - DELETE
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var rating = _unitOfWork.RatingRepository.ReadOne(id);
            if (rating == null)
            {
                return NotFound();
            }
            return View(rating);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int? id)
        {
            var rating = _unitOfWork.RatingRepository.ReadOne(id);
            if (rating == null)
            {
                return View();
            }
            _unitOfWork.RatingRepository.Delete(rating);
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

            var rating = _unitOfWork.RatingRepository.ReadOne(id);
            if (rating == null)
            {
                return NotFound();
            }

            return View(rating);
        }
    }
}
