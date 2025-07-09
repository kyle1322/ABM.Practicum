using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Timesheets_APP.Data;
using Timesheets_APP.ViewModels;

namespace Timesheets_APP.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signIn;
        private readonly ApplicationDbContext _db;

        // Modify your constructor:
        public AccountController(SignInManager<IdentityUser> signIn, ApplicationDbContext db)
        {
            _signIn = signIn;
            _db = db;
        }

      

        // ─── GET: /Account/Select
        [HttpGet]
        [AllowAnonymous]

        [HttpPost]      
        public IActionResult Select(int empId)
        {
            var emp = _db.Employees.Find(empId);
            if (emp == null)
            {
                ViewBag.ErrorMessage = "Employee ID not found";
                return View();
            }

            return RedirectToAction("Index", "Employee", new { empId });
        }

        // ─── GET: /Account/Admin
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Admin()
        {
            // returns Views/Account/Admin.cshtml
            return View(new AdminLoginViewModel());
        }

        // ─── POST: /Account/Admin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Admin(AdminLoginViewModel model, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (!ModelState.IsValid)
                return View(model);

            var result = await _signIn.PasswordSignInAsync(
                model.Username,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                // go to your Admin dashboard (AdminController.Index)
                return LocalRedirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Invalid username or password");
            return View(model);
        }

        // ─── POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signIn.SignOutAsync();
            return RedirectToAction(nameof(Select));
        }
    }
}
