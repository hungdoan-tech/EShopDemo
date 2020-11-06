using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Repository;
using Spice.Service.State;
using Spice.Utility;

namespace Spice.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class OrderController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _db;
        private readonly IUnitOfWork _unitOfWork;

        private int PageSize = 5;
        private int PageAdminSize = 10;
        public OrderController(ApplicationDbContext db, IEmailSender emailSender, IUnitOfWork unitOfWork)
        {
            _db = db;
            _emailSender = emailSender;
            _unitOfWork = unitOfWork;
        }


        [Authorize]
        public async Task<IActionResult> Confirm(int id)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderDetailsViewModel orderDetailsViewModel = new OrderDetailsViewModel()
            {
                OrderHeader = await _db.OrderHeader.Include(o => o.ApplicationUser).FirstOrDefaultAsync(o => o.Id == id && o.UserId == claim.Value),
                OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == id).ToListAsync()
            };

            return View(orderDetailsViewModel);
        }

        public IActionResult Index()
        {
            return View();
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

            

            List<OrderHeader> OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser).Where(u => u.UserId == claim.Value).ToListAsync();

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
        public async Task<IActionResult> ManageOrder(int productPage = 1)
        {

            List<OrderDetailsViewModel> orderDetailsVM = new List<OrderDetailsViewModel>();

            List<OrderHeader> OrderHeaderList = await _db.OrderHeader.Where(o => o.Status == SD.StatusSubmitted || o.Status == SD.StatusInProcess).OrderByDescending(u => u.OrderDate).ToListAsync();


            foreach (OrderHeader item in OrderHeaderList)
            {
                OrderDetailsViewModel individual = new OrderDetailsViewModel
                {
                    OrderHeader = item,
                    OrderDetails = await _db.OrderDetails.Where(o => o.OrderId == item.Id).ToListAsync()
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
                OrderHeader = await _db.OrderHeader.Include(el => el.ApplicationUser).FirstOrDefaultAsync(m => m.Id == Id),
                OrderDetails = await _db.OrderDetails.Where(m => m.OrderId == Id).ToListAsync()
            };
            orderDetailsViewModel.OrderHeader.ApplicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == orderDetailsViewModel.OrderHeader.UserId);
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



            List<OrderHeader> OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser).Where(u => u.UserId == claim.Value).ToListAsync();

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
                OrderHeader = await _db.OrderHeader.Include(el => el.ApplicationUser).FirstOrDefaultAsync(m => m.Id == Id),
                OrderDetails = await _db.OrderDetails.Where(m => m.OrderId == Id).ToListAsync()
            };
            orderDetailsViewModel.OrderHeader.ApplicationUser = await _db.ApplicationUser.FirstOrDefaultAsync(u => u.Id == orderDetailsViewModel.OrderHeader.UserId);

            return View(orderDetailsViewModel);
        }

        [Authorize]
        [HttpGet]
        [Authorize(Roles = SD.CustomerEndUser)]
        public JsonResult TrackingOrder()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            List<OrderHeader> OrderHeaderList =  _db.OrderHeader.Include(o => o.ApplicationUser).Where(u => u.UserId == claim.Value).ToList();
            var ItemCount = OrderHeaderList.Where(o => !o.Status.Equals(SD.StatusCompleted)).Count();
            return new JsonResult(ItemCount);
        }

        [Authorize(Roles =SD.RepositoryManager + ","+ SD.ManagerUser)]
        public async Task<IActionResult> OrderPrepare(int OrderId)
        {
            OrderContext orderContext = new OrderContext(_unitOfWork, _emailSender, new PreparedState());
            orderContext.ApplyState(OrderId);
            return RedirectToAction("ManageOrder", "Order");
        }


        [Authorize(Roles = SD.RepositoryManager + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderReady(int OrderId)
        {
            OrderContext orderContext = new OrderContext(_unitOfWork, _emailSender, new OnShippingState());
            orderContext.ApplyState(OrderId);
            return RedirectToAction("ManageOrder", "Order");
        }


        [Authorize(Roles = SD.RepositoryManager + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderCancel(int OrderId)
        {
            OrderContext orderContext = new OrderContext(_unitOfWork, _emailSender, new CancelledState());
            orderContext.ApplyState(OrderId);
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.RepositoryManager + "," + SD.ManagerUser)]
        public async Task<IActionResult> OrderSubmit(int OrderId)
        {
            OrderContext orderContext = new OrderContext(_unitOfWork, _emailSender, new SubmittedState());
            orderContext.ApplyState(OrderId);
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.Shipper + "," + SD.ManagerUser)]
        [Route("~/Admin/Order/OrderPickup")]
        [HttpPost]
        [ActionName("OrderPickup")]
        public async Task<IActionResult> OrderPickupPost(int OrderId)
        {
            OrderContext orderContext = new OrderContext(_unitOfWork, _emailSender, new CompletedState());
            orderContext.ApplyState(OrderId);
            return RedirectToAction("ManageOrder", "Order");
        }

        [Authorize(Roles = SD.ManagerUser + "," + SD.Shipper)]
        [Route("~/Admin/Order/OrderPickup")]
        public async Task<IActionResult> OrderPickup(int productPage = 1, string searchEmail=null, string searchPhone = null, string searchName = null)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            OrderListViewModel orderListVM = new OrderListViewModel()
            {
                Orders = new List<OrderDetailsViewModel>()
            };

            StringBuilder param = new StringBuilder();
            param.Append("/Admin/Order/OrderPickup?productPage=:");
            param.Append("&searchName=");
            if(searchName!=null)
            {
                param.Append(searchName);
            }
            param.Append("&searchEmail=");
            if (searchEmail != null)
            {
                param.Append(searchEmail);
            }
            param.Append("&searchPhone=");
            if (searchPhone != null)
            {
                param.Append(searchPhone);
            }

            List<OrderHeader> OrderHeaderList = new List<OrderHeader>();
            if (searchName != null || searchEmail != null || searchPhone != null)
            {
                var user = new ApplicationUser();

                if(searchName!=null)
                {
                    OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser)
                                                .Where(u => u.PickupName.ToLower().Contains(searchName.ToLower()))
                                                .OrderByDescending(o => o.OrderDate).ToListAsync();
                }
                else
                {
                    if (searchEmail != null)
                    {
                        user = await _db.ApplicationUser.Where(u => u.Email.ToLower().Contains(searchEmail.ToLower())).FirstOrDefaultAsync();
                        OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser)
                                                    .Where(o=>o.UserId==user.Id)
                                                    .OrderByDescending(o => o.OrderDate).ToListAsync();
                    }
                    else
                    {
                        if (searchPhone != null)
                        {
                            OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser)
                                                        .Where(u => u.PhoneNumber.Contains(searchPhone))
                                                        .OrderByDescending(o => o.OrderDate).ToListAsync();
                        }
                    }
                }
            }
            else
            {
                OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser).Where(u => u.Status == SD.StatusReady).ToListAsync();
            }

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
                urlParam = param.ToString()
            };

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

            List<OrderHeader> OrderHeaderList = await _db.OrderHeader.Include(o => o.ApplicationUser).ToListAsync();

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
    }
}