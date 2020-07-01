using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spice.Utility;

namespace Spice.Areas.Admin
{
    [Authorize(Roles = SD.ManagerUser)]
    [Area("Admin")]
    public class DashboardController : Controller
    {
        [Route("Admin")]
        [Route("Admin/Dashboard")]
        [Route("Admin/Dashboard/Index")]
        public IActionResult Index()
        {
            return View();
        }
    }
}