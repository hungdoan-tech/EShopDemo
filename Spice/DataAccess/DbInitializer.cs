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
            user = await _db.Users.FirstOrDefaultAsync(u => u.Email == "repository@gmail.com");
            await _userManager.AddToRoleAsync(user, SD.RepositoryManager);        
        }
    }
}
