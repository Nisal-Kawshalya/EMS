using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMS.Data;
using EMS.Models;

namespace EMS.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 🏠 STUDENT DASHBOARD (MY CLASSES)
        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var student = _context.Students
                .FirstOrDefault(s => s.UserId == userId);

            if (student == null)
                return RedirectToAction("Logout", "Account");

            // ✅ SEND STUDENT CODE TO VIEW
            ViewBag.StudentCode = student.StudentCode;

            var classes = _context.ClassStudents
                .Where(cs => cs.StudentId == student.Id)
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Teacher)
                .Select(cs => cs.Class)
                .ToList();

            return View(classes);
        }

        // 📘 CLASS DETAILS (READ ONLY)
        public IActionResult ClassDetails(int classId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
                return RedirectToAction("Login", "Account");

            var student = _context.Students
                .FirstOrDefault(s => s.UserId == userId);

            if (student == null)
                return RedirectToAction("Logout", "Account");

            var cls = _context.Classes
                .Include(c => c.Teacher)
                .Include(c => c.Attendences)
                .Include(c => c.Homeworks)
                .Include(c => c.Notes)
                .Include(c => c.Results)
                .FirstOrDefault(c => c.Id == classId);

            if (cls == null)
                return NotFound();

            ViewBag.StudentId = student.Id;
            return View(cls);
        }
    }
}
