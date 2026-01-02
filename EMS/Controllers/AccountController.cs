using EMS.Data;
using EMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace EMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(string email, string password, string role,
                                      string name, string phone)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(role) ||
                string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(phone))
            {
                ViewBag.Error = "All field are required";
                return View();
            }

            if (_context.Users.Any(u => u.Email == email))
            {
                ViewBag.Error = "Email already exists";
                return View();
            }

            var user = new User
            {
                Email = email,
                PasswordHash = Hash(password),
                Role = role
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            if (role == "Student")
            {
                _context.Students.Add(new Student
                {
                    UserId = user.Id,
                    StudentCode = "STD-" + Guid.NewGuid().ToString("N")[..6],
                    Name = name,
                    PhoneNumber = phone
                });
            }
            else if (role == "Teacher")
            {
                _context.Teachers.Add(new Teacher
                {
                    UserId = user.Id,
                    TeacherCode = "TCH-" + Guid.NewGuid().ToString("N")[..6],
                    Name = name,
                    PhoneNumber = phone
                });
            }
            else
            {
                ViewBag.Error = "Invalid role selected";
                return View();
            }

            _context.SaveChanges();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email and password are required";
                return View();
            }

            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user == null || user.PasswordHash != Hash(password))
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }

            // Create session
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role);

            // Redirect by role
            return user.Role == "Teacher"
                ? RedirectToAction("Index", "Home")
                : RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        private string Hash(string input)
        {
            using var sha = SHA256.Create();
            return Convert.ToBase64String(
                sha.ComputeHash(Encoding.UTF8.GetBytes(input))
            );
        }
    }
}


