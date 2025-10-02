using BusinessObjects.DTO;
using BusinessObjects.Models;
using Group03_MVC.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Services.Service;

namespace Group03_MVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // Check if user is already logged in
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                User user = await _accountService.Login(model.Username, model.Password);
                if (user != null)
                {
                    HttpContext.Session.SetString("UserId", user.Id.ToString());
                    HttpContext.Session.SetString("FullName", user.FullName ?? "User");
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("Role", user.Role);
                    HttpContext.Session.SetString("DealerId", user.DealerId?.ToString() ?? Guid.Empty.ToString()); // Thêm DealerId vào session

                    TempData["SuccessMessage"] = "Login successful!";
                    return user.Role switch
                    {
                        "customer" => RedirectToAction("Index", "Home"),
                        "admin" => RedirectToAction("Index", "Privacy"),
                        "dealer_staff" or "dealer_manager" => RedirectToAction("Index", "Home"),
                        "evm_staff" => RedirectToAction("Index", "Home"),
                        _ => RedirectToAction("Index", "Home")
                    };
                }

                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            // Check if user is already logged in
            if (HttpContext.Session.GetString("UserId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = new User
                {
                    Username = model.Username.Trim(),
                    Password = model.Password,
                    Email = model.Email.Trim(),
                    FullName = model.FullName.Trim(),
                    Phone = model.Phone?.Trim(),
                    Role = "customer",
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                bool userExists = await _accountService.CheckUserExists(user.Username, user.Email);
                if (userExists)
                {
                    ModelState.AddModelError(string.Empty, "Username or Email already exists.");
                    return View(model);
                }

                bool result = await _accountService.Register(user);
                if (!result)
                {
                    ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
                    return View(model);
                }

                TempData["SuccessMessage"] = "Registration successful! Please login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            try
            {
                var userId = HttpContext.Session.GetString("UserId");
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login");
                }

                var user = await _accountService.GetUserById(Guid.Parse(userId));
                if (user == null)
                {
                    return NotFound();
                }

                var userDTO = new UserDTO
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    Role = user.Role
                };

                return View(userDTO);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error loading profile.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(UserDTO userDTO)
        {
                try
            {
                if (!ModelState.IsValid)
                {
                    return View("Profile", userDTO);
                }

                var result = await _accountService.UpdateUser(userDTO);
                if (result)
                {
                    HttpContext.Session.SetString("FullName", userDTO.FullName);
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to update profile.";
                }

                return RedirectToAction("Profile");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while updating profile.";
                return View("Profile", userDTO);
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LogoutPost()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        
    }
}