using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Spice.Models;
using Spice.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;                
            }

            if (_db.Roles.Any(r => r.Name == SD.ManagerUser)) return;

            var managerRoleResult = _roleManager.CreateAsync(new IdentityRole(SD.ManagerUser)).GetAwaiter().GetResult();
            var repositoryRoleResult = _roleManager.CreateAsync(new IdentityRole(SD.RepositoryManager)).GetAwaiter().GetResult();
            var shipperRoleResult = _roleManager.CreateAsync(new IdentityRole(SD.Shipper)).GetAwaiter().GetResult();
            var enduserRoleResult = _roleManager.CreateAsync(new IdentityRole(SD.CustomerEndUser)).GetAwaiter().GetResult();
            
            Thread.Sleep(2000);

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
                UserName = "admin2@gmail.com",
                Email = "admin2@gmail.com",
                Name = "Admin",
                EmailConfirmed = true,
                PhoneNumber = "1112223333"
            }, "Admin123*").GetAwaiter().GetResult();
            user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "admin2@gmail.com");
            await _userManager.AddToRoleAsync(user, SD.ManagerUser);

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
                UserName = "repository2@gmail.com",
                Email = "repository2@gmail.com",
                Name = "Repository Manager",
                EmailConfirmed = true,
                PhoneNumber = "1112223333"
            }, "Admin123*").GetAwaiter().GetResult();
            user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "repository2@gmail.com");
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

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "shipper2@gmail.com",
                Email = "shipper2@gmail.com",
                Name = "Shipper",
                EmailConfirmed = true,
                PhoneNumber = "1112223333"
            }, "Admin123*").GetAwaiter().GetResult();
            user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "shipper2@gmail.com");
            await _userManager.AddToRoleAsync(user, SD.Shipper);

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "17110154@student.hcmute.edu.vn",
                Email = "17110154@student.hcmute.edu.vn",
                Name = "Normal User",
                EmailConfirmed = true,
                PhoneNumber = "1112223333"
            }, "Admin123*").GetAwaiter().GetResult();
            user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "17110154@student.hcmute.edu.vn");
            await _userManager.AddToRoleAsync(user, SD.CustomerEndUser);
        }
    }
}
