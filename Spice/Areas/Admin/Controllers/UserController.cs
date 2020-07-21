using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Repository;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.ManagerUser)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IUnitOfWork UnitOfWork)
        {
            _unitOfWork = UnitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            return View( _unitOfWork.ApplicationUserRepository.Get(filter: u => u.Id != claim.Value));
        }


        public IActionResult Lock(string id)
        {
            if(id==null)
            {
                return NotFound();
            }

            var applicationUser = _unitOfWork.ApplicationUserRepository.ReadOneByStringID(id);

            if(applicationUser==null)
            {
                return NotFound();
            }

            applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);

            _unitOfWork.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult UnLock(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser =  _unitOfWork.ApplicationUserRepository.ReadOneByStringID(id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            applicationUser.LockoutEnd = DateTime.Now;

            _unitOfWork.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}