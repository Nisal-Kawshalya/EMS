using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMS.Controllers
{
    public class ParentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParentController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(string studentCode)
        {
            studentCode = (studentCode ?? "").Trim();

            var student = _context.Students
                .Include(s => s.ClassStudents!)
                    .ThenInclude(cs => cs.Class)
                        .ThenInclude(c => c!.Teacher)
                .FirstOrDefault(s => s.StudentCode == studentCode);

            if (student == null)
            {
                ViewBag.Error = "Student code not found.";
                return View();
            }

            return View("StudentView", student);
        }

        [HttpGet]
        public IActionResult Class(string studentCode, int classId, string tab = "Homework")
        {
            studentCode = (studentCode ?? "").Trim();

            var student = _context.Students
                .Include(s => s.ClassStudents!)
                    .ThenInclude(cs => cs.Class)
                        .ThenInclude(c => c!.Teacher)
                .FirstOrDefault(s => s.StudentCode == studentCode);

            if (student == null) return RedirectToAction("Index");

            var enrolled = _context.ClassStudents.Any(cs => cs.StudentId == student.Id && cs.ClassId == classId);
            if (!enrolled) return Forbid();

            var cls = _context.Classes
                .Include(c => c.Teacher)
                .Include(c => c.Homeworks)
                .Include(c => c.Notes)
                .FirstOrDefault(c => c.Id == classId);

            if (cls == null) return NotFound();

            var attendance = _context.Attendences
                .Where(a => a.ClassId == classId && a.StudentId == student.Id)
                .OrderByDescending(a => a.Date)
                .ToList();

            var results = _context.Results
                .Where(r => r.ClassId == classId && r.StudentId == student.Id)
                .OrderByDescending(r => r.ExamDate)
                .ToList();

            ViewBag.Student = student;
            ViewBag.StudentCode = student.StudentCode;
            ViewBag.ActiveTab = tab;
            ViewBag.Attendance = attendance;
            ViewBag.Results = results;

            return View("Class", cls);
        }
    }
}
