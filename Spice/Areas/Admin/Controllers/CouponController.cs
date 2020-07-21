using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Repository;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class CouponController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CouponController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _unitOfWork.CouponRepository.ReadAll().ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Coupon coupons)
        {
            if(ModelState.IsValid)
            {
                _unitOfWork.CouponRepository.Create(coupons);
                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(coupons);
        }


        //GET Edit Coupon
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = _unitOfWork.CouponRepository.ReadOne(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Coupon coupons)
        {
            if (coupons.Id == 0)
            {
                return NotFound();
            }

            var couponFromDb = _unitOfWork.CouponRepository.ReadOne(coupons.Id);

            if (ModelState.IsValid)
            {
                couponFromDb.MinimumAmount = coupons.MinimumAmount;
                couponFromDb.Name = coupons.Name;
                couponFromDb.Discount = coupons.Discount;
                couponFromDb.CouponType = coupons.CouponType;
                couponFromDb.IsActive = coupons.IsActive;

                _unitOfWork.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(coupons);
        }

        //GET Delete Coupon
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var coupon = _unitOfWork.CouponRepository.ReadOne(id);
            if (coupon == null)
            {
                return NotFound();
            }
            return View(coupon);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var coupons = _unitOfWork.CouponRepository.ReadOne(id);
            _unitOfWork.CouponRepository.Delete(coupons);
            _unitOfWork.CouponRepository.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coupon = _unitOfWork.CouponRepository.ReadOne(id);
            if (coupon == null)
            {
                return NotFound();
            }

            return View(coupon);
        }
    }
}