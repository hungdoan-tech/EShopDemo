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
using Spice.Repository;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class NewsPostController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public NewsPostController(IUnitOfWork UnitOfWork)
        {
            _unitOfWork = UnitOfWork;
        }
        //[Route("NewsPost/{Alias}-{Id}")]
        public IActionResult NewsPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = _unitOfWork.NewsRepository.ReadOneNewsIncludeMenuItem(id);

            return View(news);
        }
        public IActionResult Index()
        {

            var news = _unitOfWork.NewsRepository.ReadAllCouponOrNews().ToList();
            return View(news);
        }
    }
}
