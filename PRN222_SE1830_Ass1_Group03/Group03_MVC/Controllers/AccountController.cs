using BusinessObjects.Models;
using Group03_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Services;

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
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = accountService.Login(model.Username, model.Password).Result;
            if(user != null)
            {
                return RedirectToAction("Index", "Home");
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

            Console.WriteLine("USERNAME: "+ model.Username);
            Console.WriteLine("EMAIL: " + model.Email);
            Console.WriteLine("PASSWORD: " + model.Password);
            Console.WriteLine("FULLNAME: " + model.FullName);
            Console.WriteLine("PHONE: " + model.Phone);

            bool userExists = await accountService.CheckUserExists(user.Username, user.Email);
            if (userExists)
            {
                ModelState.AddModelError(string.Empty, "Username or Email already exists.");
                return View(model);
            }
            accountService.Register(user);
            return RedirectToAction("Login");
        }
    }
}



