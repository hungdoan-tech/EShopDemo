using System.Linq;
using Microsoft.AspNetCore.Mvc;
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
