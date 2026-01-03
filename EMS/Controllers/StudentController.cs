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

        // 🏠 STUDENT DASHBOARD → ALL CLASSES (NEW IDEA)
        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            // ✅ FETCH ALL CLASSES CREATED BY ALL TEACHERS
            var classes = _context.Classes
                .Include(c => c.Teacher)
                .ToList();

            return View(classes);
        }

        // 📘 CLASS DETAILS (READ ONLY)
        public IActionResult ClassDetails(int classId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Account");

            var cls = _context.Classes
                .Include(c => c.Teacher)
                .Include(c => c.Attendences)
                .Include(c => c.Homeworks)
                .Include(c => c.Notes)
                .Include(c => c.Results)
                .FirstOrDefault(c => c.Id == classId);

            if (cls == null)
                return NotFound();

            return View(cls);
        }
    }
}
