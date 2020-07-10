using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.ManagerUser +","+ SD.RepositoryManager)]
    [Area("Admin")]
    public class ImportController : Controller
    {
        public IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public ImportController(IHttpContextAccessor httpContextAccessor,ApplicationDbContext applicationDbContext, IWebHostEnvironment webHostEnvironment)
        {
            _httpContextAccessor = httpContextAccessor;
            _db = applicationDbContext;
            _hostingEnvironment = webHostEnvironment;
        }
        
        public IActionResult Index()
        {
            var listImportHistory = _db.ImportHistories.ToList();
            return View(listImportHistory);
        }

        [HttpPost]
        public IActionResult ImportProduct()
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            int importID = 0;
            if (_db.ImportHistories.Count() > 0)
            {
                importID = _db.ImportHistories.OrderByDescending(a => a.Id).Take(1).Last().Id + 1;
            }
            string filepath = null;
            if (files.Count > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "Excel");
                var extension = Path.GetExtension(files[0].FileName);
                string path = Path.Combine(uploads, "Excel" + importID + extension);
                filepath = path;
                FileInfo file = new FileInfo(path);
                using (var filesStream = new FileStream(Path.Combine(uploads, "Excel"+ importID + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                using (var stream = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        int index = 0;
                        while (reader.Read())
                        {
                            if (index >= 1)
                            {
                                int[] Values = { 0,0,0 };                             
                                for (int i = 0; i < 3; i++)
                                {
                                    var item = reader.GetValue(i);
                                    if (item !is Int32)                                    
                                    {
                                        ViewBag.Result = "Wrong Type - Not Read All The Line Of This File";
                                        return RedirectToAction(nameof(Index));
                                    }
                                    Values[i] = Convert.ToInt32(item);
                                }
                                try
                                {
                                    var subCategoryID = _db.SubCategory.First(a => a.Id == Values[0]).Id;
                                    var menuItem = _db.MenuItem.First(a => a.Id == Values[1]);
                                    var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                                    _db.ImportHistories.Add(new Models.ImportHistory() { SubCategoryId = Values[0], MenuItemID = Values[1], Quantity = Values[2], UserId = userId, ImportDate = DateTime.Now});
                                    menuItem.Quantity += Values[2];
                                    _db.SaveChanges();
                                }
                                catch
                                {
                                    ViewBag.Result = "Wrong Type - Not Read All The Line Of This File";
                                    return RedirectToAction(nameof(Index));
                                }
                            }
                            index++;
                        }
                    }
                }
                ViewBag.Result = "Importing Product Completed";
            }
            if (filepath != null)
            {
                FileInfo tempfile = new FileInfo(filepath);
                tempfile.Delete();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}