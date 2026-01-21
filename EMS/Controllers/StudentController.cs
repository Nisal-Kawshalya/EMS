using EMS.Data;
using EMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public StudentController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // ✅ STUDENT DASHBOARD → ONLY ENROLLED CLASSES
        public IActionResult Dashboard()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Student")
                return RedirectToAction("Login", "Account");

            // ✅ Load student with ALL related data
            var student = _context.Students
                .Include(s => s.User)
                .Include(s => s.ClassStudents)
                    .ThenInclude(cs => cs.Class)
                        .ThenInclude(c => c.Teacher)
                .Include(s => s.Attendences)
                .Include(s => s.Results)
                .FirstOrDefault(s => s.UserId == userId.Value);

            if (student == null)
                return RedirectToAction("Login", "Account");

            return View(student);
        }

        // ✅ STUDENT PROFILE (shows StudentCode + Profile Photo)
        [HttpGet]
        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Student")
                return RedirectToAction("Login", "Account");

            var student = _context.Students.FirstOrDefault(s => s.UserId == userId.Value);
            if (student == null) return RedirectToAction("Login", "Account");

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadProfilePhoto(IFormFile photo)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Student")
                return RedirectToAction("Login", "Account");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId.Value);
            if (student == null) return RedirectToAction("Login", "Account");

            if (photo == null || photo.Length == 0)
            {
                TempData["Error"] = "Please select an image file.";
                return RedirectToAction("Profile");
            }

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(photo.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
            {
                TempData["Error"] = "Only JPG, PNG, or WebP files are allowed.";
                return RedirectToAction("Profile");
            }

            if (photo.Length > 2 * 1024 * 1024)
            {
                TempData["Error"] = "Image size must be 2MB or less.";
                return RedirectToAction("Profile");
            }

            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "students");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"stu_{student.Id}_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsDir, fileName);

            await using (var stream = System.IO.File.Create(fullPath))
            {
                await photo.CopyToAsync(stream);
            }

            // delete old photo if exists
            if (!string.IsNullOrWhiteSpace(student.ProfileImagePath))
            {
                var oldPhysical = Path.Combine(_env.WebRootPath, student.ProfileImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(oldPhysical))
                    System.IO.File.Delete(oldPhysical);
            }

            student.ProfileImagePath = $"/uploads/students/{fileName}";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Profile photo updated.";
            return RedirectToAction("Profile");
        }

        // 📘 CLASS PAGE (Homework / Notes / Results) - keep your logic
        public IActionResult Class(int classId, string tab = "Homework")
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            string? role = HttpContext.Session.GetString("Role");

            if (userId == null || role != "Student")
                return RedirectToAction("Login", "Account");

            var student = _context.Students.FirstOrDefault(s => s.UserId == userId.Value);
            if (student == null) return RedirectToAction("Login", "Account");

            // ✅ Security: student can open only enrolled class
            bool enrolled = _context.ClassStudents.Any(cs => cs.StudentId == student.Id && cs.ClassId == classId);
            if (!enrolled) return Forbid();

            var cls = _context.Classes
                .Include(c => c.Teacher)
                .Include(c => c.Homeworks)
                .Include(c => c.Notes)
                .Include(c => c.Results)
                .FirstOrDefault(c => c.Id == classId);

            if (cls == null)
                return NotFound();

            ViewBag.ActiveTab = tab;
            return View(cls);
        }
    }
}
