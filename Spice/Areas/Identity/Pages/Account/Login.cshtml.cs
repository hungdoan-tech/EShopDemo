using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Microsoft.EntityFrameworkCore;
using Spice.Models;
using Microsoft.AspNetCore.Http;
using Spice.Utility;
using System.Threading;

namespace Spice.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public LoginModel(SignInManager<IdentityUser> signInManager, ILogger<LoginModel> logger, ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true

                ApplicationUser currentUser = _db.ApplicationUser.First(a => a.Email == Input.Email.Trim());
                if (currentUser==null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }                
                var result = await _signInManager.PasswordSignInAsync(currentUser.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                var users = await _signInManager.UserManager.FindByEmailAsync(Input.Email);

                var isInRoleAdmin = await _signInManager.UserManager.IsInRoleAsync(users, SD.ManagerUser);
                var isInRoleShipper = await _signInManager.UserManager.IsInRoleAsync(users, SD.Shipper);
                var isInRoleReManager = await _signInManager.UserManager.IsInRoleAsync(users, SD.RepositoryManager);
                if (result.Succeeded)
                {
                    if (isInRoleAdmin || isInRoleReManager || isInRoleShipper)
                    {
                        returnUrl = returnUrl ?? Url.Content("~/Admin");

                    }
                    else
                    {
                        returnUrl = returnUrl ?? Url.Content("~/");
                    }
                    var user = await _db.Users.Where(u => u.Email == Input.Email).FirstOrDefaultAsync();
                    _logger.LogInformation("User logged in.");


                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
