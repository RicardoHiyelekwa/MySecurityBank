using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySecurityBank.Models;
using MySecurityBank.Models.MySecureBank.Models;
using System.Security.Claims;
using BCrypt.Net;
namespace MySecurityBank.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext db, ILogger<AccountController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            // Basic server-side validation to prevent simple injection attempts
            if (model.FullName.Contains("<") || model.FullName.Contains(">"))
            {
                ModelState.AddModelError("", "Invalid characters in name.");
                return View(model);
            }

            var existing = await _db.Customers.FirstOrDefaultAsync(c => c.AccountNumber == model.AccountNumber);
            if (existing != null)
            {
                ModelState.AddModelError("", "Account already exists.");
                return View(model);
            }

            var hash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            var customer = new Customer
            {
                FullName = model.FullName.Trim(),
                IdNumber = model.IdNumber.Trim(),
                AccountNumber = model.AccountNumber.Trim(),
                PasswordHash = hash
            };
            _db.Customers.Add(customer);
            await _db.SaveChangesAsync();

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.AccountNumber == model.AccountNumber);
            if (customer == null || !BCrypt.Net.BCrypt.Verify(model.Password, customer.PasswordHash))
            {
                ModelState.AddModelError("", "Invalid credentials.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Name, customer.FullName),
                new Claim("AccountNumber", customer.AccountNumber)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
