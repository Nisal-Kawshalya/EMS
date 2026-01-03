using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMS.Data;

namespace EMS.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🏠 STUDENT DASHBOARD → ALL CLASSES
        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Student")
                return RedirectToAction("Login", "Account");

            var classes = _context.Classes
                .Include(c => c.Teacher)
                .OrderByDescending(c => c.Id)
                .ToList();

            return View(classes);
        }

        // 📘 CLASS PAGE (Homework / Notes / Results)
        public IActionResult Class(int classId, string tab = "Homework")
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Student")
                return RedirectToAction("Login", "Account");

            var cls = _context.Classes
                .Include(c => c.Teacher)
                .Include(c => c.Homeworks)
                .Include(c => c.Notes)
                .Include(c => c.Results)
                .FirstOrDefault(c => c.Id == classId);

            if (cls == null)
                return NotFound();

            ViewBag.ActiveTab = tab;
            return View(cls); // 👉 loads Views/Student/Class.cshtml
        }
    }
}
