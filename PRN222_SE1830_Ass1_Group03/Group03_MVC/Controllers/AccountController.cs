using BusinessObjects.Models;
using Group03_MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Services;
using System.Security.Claims;

namespace Group03_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = await accountService.Login(model.Username, model.Password);

            if (user != null)
            {
                s

                if (user.Role == "customer")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (user.Role == "admin")
                {
                    return RedirectToAction("Index", "Privacy");
                }
                else if (user.Role == "dealer_staff" || user.Role == "dealer_manager")
                {
                    return RedirectToAction("Index", "Home");
                }
                else if (user.Role == "evm_staff")
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Password = model.Password,
                Email = model.Email,
                FullName = model.FullName,
                Phone = model.Phone,
                Role = "customer", 

            };

            bool userExists = await accountService.CheckUserExists(user.Username, user.Email);
            if (userExists)
            {
                ModelState.AddModelError(string.Empty, "Username or Email already exists.");
                return View(model);
            }
            accountService.Register(user);
            return RedirectToAction("Login");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("LoginCookie");
            return RedirectToAction("Login", "Account");
        }
    }
}



