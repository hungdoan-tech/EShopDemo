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


        public async Task Initialize()
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

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "repository@gmail.com",
                Email = "repository@gmail.com",
                Name = "Repository Manager",
                EmailConfirmed = true,
                PhoneNumber = "1112223333"
            }, "Admin123*").GetAwaiter().GetResult();
            IdentityUser user2 = await _db.Users.FirstOrDefaultAsync(u => u.Email == "repository@gmail.com");
            await _userManager.AddToRoleAsync(user2, SD.RepositoryManager);

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "17110154@student.hcmute.edu.vn",
                Email = "17110154@student.hcmute.edu.vn",
                Name = "Normal User",
                EmailConfirmed = true,
                PhoneNumber = "1112223333"
            }, "Admin123*").GetAwaiter().GetResult();
            IdentityUser user3 = await _db.Users.FirstOrDefaultAsync(u => u.Email == "17110154@student.hcmute.edu.vn");
            await _userManager.AddToRoleAsync(user3, SD.CustomerEndUser);

            //_userManager.CreateAsync(new ApplicationUser
            //{
            //    UserName = "shipper@gmail.com",
            //    Email = "shipper@gmail.com",
            //    Name = "Shipper",
            //    EmailConfirmed = true,
            //    PhoneNumber = "1112223333"
            //}, "Admin123*").GetAwaiter().GetResult();
            //IdentityUser user3 = await _db.Users.FirstOrDefaultAsync(u => u.Email == "shipper@gmail.com");
            //await _userManager.AddToRoleAsync(user3, SD.Shipper);       
        }
    }
}
