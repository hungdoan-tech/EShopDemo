using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spice.Models;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Data
{
    public class DbInitializer : IDbInitializer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitializer(ApplicationDbContext db, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }


        public async void Initialize()
        {
            try
            {
                if(_db.Database.GetPendingMigrations().Count()>0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            if (_db.Roles.Any(r => r.Name == SD.ManagerUser)) return;

            _roleManager.CreateAsync(new IdentityRole(SD.ManagerUser)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.RepositoryManager)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.Shipper)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                Name = "Admin",
                EmailConfirmed = true,
                PhoneNumber = "1112223333"
            }, "Admin123*").GetAwaiter().GetResult();

            IdentityUser user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");
            await _userManager.AddToRoleAsync(user, SD.ManagerUser);
            _db.SaveChanges();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "repository@gmail.com",
                Email = "repository@gmail.com",
                Name = "Repository Manager",
                EmailConfirmed = true,
                PhoneNumber = "1112223333"
            }, "Admin123*").GetAwaiter().GetResult();
            user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "repository@gmail.com");
            await _userManager.AddToRoleAsync(user, SD.RepositoryManager);

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "shipper@gmail.com",
                Email = "shipper@gmail.com",
                Name = "Shipper",
                EmailConfirmed = true,
                PhoneNumber = "1112223333"
            }, "Admin123*").GetAwaiter().GetResult();
            user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "shipper@gmail.com");
            await _userManager.AddToRoleAsync(user, SD.Shipper);
            _db.SaveChanges();

            //            List<Category> listCategory = new List<Category>()
            //            {
            //                new Category{Name="SmartWatch"},
            //                new Category{Name="Analog"}
            //            };
            //            _db.Category.AddRange(listCategory);

            //            List<SubCategory> listSubCategory = new List<SubCategory>()
            //            {
            //                new SubCategory{Name="Apple"},
            //                new SubCategory{Name="Casio"},
            //                new SubCategory{Name="Rolex"},
            //                new SubCategory{Name="Samsung"}
            //            };
            //            _db.SubCategory.AddRange(listSubCategory);

            //            List<Coupon> listCoupon = new List<Coupon>()
            //            {
            //                new Coupon{Name="15OFF",CouponType="0",Discount=15,MinimumAmount=75,IsActive=true}
            //            };
            //            _db.Coupon.AddRange(listCoupon);

            //            List<News> listNews = new List<News>()
            //            {
            //                new News{
            //                    Header="Sale on for everything 15OFF",Content = "<p>In this summer, we have a coupon for everything for 15 % each deal which is larger than 50 $&nbsp;</p><p><img alt="+"15 Off Images, Stock Photos &amp; Vectors | Shutterstock"+"src="+"https://image.shutterstock.com/image-vector/special-offer-15-off-label-260nw-1109101598.jpg" +"/></p>"
            //+"<div class="+"eJOY__extension_root_class" +"id="+"eJOY__extension_root"+ "style="+"all:unset"+">&nbsp;</div>", Alias="Sale-Off-15OFF", PublishedDate=DateTime.Now, Type="1", ImageHeader="\\images\\News1.png"
            //                }
            //            };
            //            _db.Coupon.AddRange(listCoupon);


            //            List<MenuItem> listMenuItems = new List<MenuItem>()
            //            {
            //                new MenuItem{Name="Rolex 1", Description="Awesome",Image = "\\images\\13.png",Price=100,IsPublish=true,Quantity=6,Color="3",Tag="2",PublishedDate = DateTime.Now, CategoryId=2, SubCategoryId=3},
            //                new MenuItem{Name="Rolex 2", Description="Awesome",Image = "\\images\\14.png",Price=156,IsPublish=true,Quantity=20,Color="3",Tag="2",PublishedDate = DateTime.Now, CategoryId=2, SubCategoryId=3},
            //                new MenuItem{Name="Rolex 3", Description="Awesome",Image = "\\images\\15.png",Price=25,IsPublish=true,Quantity=23,Color="3",Tag="2",PublishedDate = DateTime.Now, CategoryId=2, SubCategoryId=3},
            //                new MenuItem{Name="Casio 1", Description="Awesome",Image = "\\images\\16.png",Price=245,IsPublish=true,Quantity=20,Color="1",Tag="2",PublishedDate = DateTime.Now, CategoryId=1, SubCategoryId=2},
            //                new MenuItem{Name="Casio 2", Description="Awesome",Image = "\\images\\17.png",Price=154,IsPublish=true,Quantity=25,Color="1",Tag="1",PublishedDate = DateTime.Now, CategoryId=2, SubCategoryId=2},
            //                new MenuItem{Name="Casio 3", Description="Awesome",Image = "\\images\\18.png",Price=157,IsPublish=true,Quantity=15,Color="1",Tag="1",PublishedDate = DateTime.Now, CategoryId=2, SubCategoryId=2},
            //                new MenuItem{Name="Samsung 1", Description="Awesome",Image = "\\images\\20.png",Price=198,IsPublish=true,Quantity=23,Color="3",Tag="0",PublishedDate = DateTime.Now, CategoryId=1, SubCategoryId=3},
            //                new MenuItem{Name="Apple 1", Description="Awesome",Image = "\\images\\21.png",Price=998,IsPublish=true,Quantity=18,Color="1",Tag="0",PublishedDate = DateTime.Now, CategoryId=1, SubCategoryId=1},
            //            };
            //            _db.MenuItem.AddRange(listMenuItems);

            //            _db.SaveChanges();
        }
    }
}
