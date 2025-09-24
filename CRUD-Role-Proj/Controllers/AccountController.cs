
using CRUD_Role_Proj.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CRUD_Role_Proj.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        //test
        public async Task<IActionResult> TestUsers()
        {
            var allUsers = await _context.Users.Include(u => u.Role).ToListAsync();
            return Json(allUsers.Select(u => new { u.Username, u.PasswordHash, Role = u.Role.Name }));
        }


        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            var users = _context.Users;
            return View(users);
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter username and password.";
                return View();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == password);

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            // Build claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Name)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Redirect based on role
            switch (user.Role.Name)
            {
                case "Admin":
                    return RedirectToAction("Index", "Customers");
                case "Manager":
                    return RedirectToAction("Index", "Customers");
                case "Employee":
                    return RedirectToAction("Index", "Customers");
                default:
                    return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // GET: /Account/Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var roles = await _context.Roles
                .OrderBy(r => r.Id)
                .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
                .ToListAsync();
            ViewBag.Roles = roles;
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, int roleId)
        {
            // Basic validation
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Please enter username and password.";
                var roles = await _context.Roles
                    .OrderBy(r => r.Id)
                    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
                    .ToListAsync();
                ViewBag.Roles = roles;
                return View();
            }

            var exists = await _context.Users.AnyAsync(u => u.Username == username);
            if (exists)
            {
                ViewBag.Error = "This user already has an account.";
                var roles = await _context.Roles
                    .OrderBy(r => r.Id)
                    .Select(r => new SelectListItem { Value = r.Id.ToString(), Text = r.Name })
                    .ToListAsync();
                ViewBag.Roles = roles;
                return View();
            }

            var user = new User
            {
                Username = username,
                PasswordHash = password,
                RoleId = roleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Sign-in after registration
            var role = await _context.Roles.FindAsync(roleId);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, role?.Name ?? "")
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties { IsPersistent = true });

            return RedirectToAction("Index", "Customers");
        }
    }
}
