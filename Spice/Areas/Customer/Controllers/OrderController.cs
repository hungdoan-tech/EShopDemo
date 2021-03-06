﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Reports;
using Spice.Repository;
using Spice.Service;
using Spice.Service.ServiceInterfaces;
using Spice.Service.State;
using Spice.Utility;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IOrderContext _orderContext;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<IdentityUser> _userManager;


        private readonly int PageSize = 5;
        private readonly int PageAdminSize = 10;
        public OrderController(ApplicationDbContext db, IOrderContext orderContext, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
        {
            this._db = db;
            this._orderContext = orderContext;
            this._webHostEnvironment = webHostEnvironment;
            this._userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await _db.OrderHeader.Include(o => o.Customer).FirstOrDefaultAsync(o => o.Id == id && o.UserId == claim.Value),
                OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == id).ToListAsync()
            };

            return View(orderDetailsViewModel);
        }

        public IActionResult GetOrderStatus(int Id)
        {
            return PartialView("_OrderStatus", _db.OrderHeader.Where(m => m.Id == Id).FirstOrDefault().Status);
        }

        [Authorize(Roles = SD.CustomerEndUser)]
        public async Task<IActionResult> OrderHistory(int productPage=1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            

            List<OrderHeader> OrderHeaderList = await _db.OrderHeader.Include(o => o.Customer).Where(u => u.UserId == claim.Value).ToListAsync();

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderListVM.Orders.Add(individual);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(p => p.OrderHeader.Id)
                                 .Skip((productPage - 1) * PageSize)
                                 .Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                urlParam = "/Customer/Order/OrderHistory?productPage=:"
            };

            return View(orderListVM);
        }

        [Authorize(Roles = SD.RepositoryManager + "," + SD.ManagerUser)]
        [Route("~/Admin/Order/ManageOrder")]
        public async Task<IActionResult> ManageOrder()
        {

            List<OrderDetailsViewModel> orderDetailsVM = new List<OrderDetailsViewModel>();

            List<OrderHeader> OrderHeaderList = await _db.OrderHeader.Where(o => o.Status == SD.StatusSubmitted || o.Status == SD.StatusInProcess).OrderByDescending(u => u.OrderDate).ToListAsync();

            var temp = await _userManager.GetUsersInRoleAsync(SD.Shipper);

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync(),
                    lstShipper = (List<IdentityUser>)temp
                };
                orderDetailsVM.Add(individual);
            }

            return View(orderDetailsVM.OrderBy(o => o.OrderHeader.OrderDate).ToList());
        }

        [Authorize(Roles = SD.CustomerEndUser + "," + SD.ManagerUser + "," +SD.Shipper)]
        public async Task<IActionResult> GetOrderDetails(int Id)
        {
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await _db.OrderHeader.Include(el => el.Customer).FirstOrDefaultAsync(m => m.Id == Id),
                OrderDetails = await _db.OrderDetails.Where(m => m.OrderId == Id).ToListAsync()
            };
            orderDetailsViewModel.OrderHeader.Customer = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == orderDetailsViewModel.OrderHeader.UserId);
            return PartialView("_IndividualOrderDetails", orderDetailsViewModel);
        }


        [Authorize(Roles = SD.CustomerEndUser)]
        public async Task<IActionResult> OrderTracking(int productPage = 1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };



            List<OrderHeader> OrderHeaderList = await _db.OrderHeader.Include(o => o.Customer).Where(u => u.UserId == claim.Value).ToListAsync();

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderListVM.Orders.Add(individual);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(p => p.OrderHeader.Id)
                                 .Skip((productPage - 1) * PageSize)
                                 .Take(PageSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItem = count,
                urlParam = "/Customer/Order/OrderHistory?productPage=:"
            };

            return View(orderListVM);
        }

        [Authorize(Roles = SD.CustomerEndUser + "," + SD.ManagerUser)]
        [Route("~/Order/DetailTracking/{id}")]
        public async Task<IActionResult> DetailTracking(int Id)
        {
            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await _db.OrderHeader.Include(el => el.Customer).FirstOrDefaultAsync(m => m.Id == Id),
                OrderDetails = await _db.OrderDetails.Where(m => m.OrderId == Id).ToListAsync()
            };
            orderDetailsViewModel.OrderHeader.Customer = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == orderDetailsViewModel.OrderHeader.UserId);

            return View(orderDetailsViewModel);
        }

        [Authorize]
        [HttpGet]
        [Authorize(Roles = SD.CustomerEndUser)]
        public JsonResult TrackingOrder()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<OrderHeader> OrderHeaderList =  _db.OrderHeader.Include(o => o.Customer).Where(u => u.UserId == claim.Value).ToList();
            var ItemCount = OrderHeaderList.Where(o => !o.Status.Equals(SD.StatusCompleted)).Count();
            return new JsonResult(ItemCount);
        }

        [Authorize(Roles =SD.RepositoryManager + ","+ SD.ManagerUser)]
        public IActionResult OrderPrepare(int OrderId)
        {
            this._orderContext.SetState(new PreparedState());
            this._orderContext.ApplyState(OrderId);
            return RedirectToAction("ManageOrder", "Order");
        }


        [Authorize(Roles = SD.RepositoryManager + "," + SD.ManagerUser)]
        public IActionResult OrderReady(int OrderId, string ShipperId)
        {
            var chosenOrder = _db.OrderHeader.First(a => a.Id == OrderId);
            chosenOrder.ShipperId = ShipperId;
            _db.SaveChanges();
            this._orderContext.SetState(new OnShippingState());
            this._orderContext.ApplyState(OrderId);
            return RedirectToAction("ManageOrder", "Order");
        }


        [Authorize(Roles = SD.RepositoryManager + "," + SD.ManagerUser)]
        public IActionResult OrderCancel(int OrderId)
        {
            this._orderContext.SetState(new CancelledState());
            this._orderContext.ApplyState(OrderId);
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.RepositoryManager + "," + SD.ManagerUser)]
        public IActionResult OrderSubmit(int OrderId)
        {
            this._orderContext.SetState(new SubmittedState());
            this._orderContext.ApplyState(OrderId);            
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.Shipper + "," + SD.ManagerUser)]
        [Route("~/Admin/Order/OrderPickup")]
        [HttpPost]
        [ActionName("OrderPickup")]
        public IActionResult OrderPickupPost(int OrderId)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var orderHeader = _db.OrderHeader.Where(m=>m.Id == OrderId).First();

            if (files.Count > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads, "Signature" + orderHeader.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                orderHeader.CustomerSignature = @"\images\" + "Signature" + orderHeader.Id + extension;
            }
            else
            {
                return RedirectToAction("OrderPickup", "Order");
            }

            _db.SaveChanges();

            this._orderContext.SetState(new CompletedState());
            this._orderContext.ApplyState(OrderId);            
            return RedirectToAction("OrderPickup", "Order");
        }

        [Authorize(Roles = SD.ManagerUser + "," + SD.Shipper)]
        [Route("~/Admin/Order/OrderPickup")]
        public async Task<IActionResult> OrderPickup([FromServices] IUserService userService)
        {
            var userId = userService.GetUserId();

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            var OrderHeaderList = await _db.OrderHeader.Include(o => o.Customer).Where(u => u.Status == SD.StatusReady && u.ShipperId == userId).ToListAsync();
            
            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderListVM.Orders.Add(individual);
            }
            return View(orderListVM);
        }



        [Authorize(Roles =  SD.ManagerUser)]
        [Route("~/Admin/Order/OrderHistoryAdmin")]
        public async Task<IActionResult> OrderHistoryAdmin(int productPage = 1)
        {
            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            List<OrderHeader> OrderHeaderList = await _db.OrderHeader.Include(o => o.Customer).ToListAsync();

            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
                };
                orderListVM.Orders.Add(individual);
            }

            var count = orderListVM.Orders.Count;
            orderListVM.Orders = orderListVM.Orders.OrderByDescending(p => p.OrderHeader.Id)
                                 .Skip((productPage -1) * PageAdminSize)
                                 .Take(PageAdminSize).ToList();

            orderListVM.PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageAdminSize,
                TotalItem = count,
                urlParam = "/Admin/Order/OrderHistoryAdmin?productPage=:"
            };

            return View(orderListVM);
        }

        [Authorize]
        public ActionResult PrintOrder(int id, string userId)
        {
            List<OrderDetails> orderDetails = _db.OrderDetails.Include(n=>n.OrderHeader).Include(n=>n.MenuItem).Where(m => m.OrderId == id).Where(m=>m.OrderHeader.UserId == userId).ToList();
            OrderReport rpt = new OrderReport(_webHostEnvironment);
            return File(rpt.Report(orderDetails), "application/pdf");
        }
    }
}