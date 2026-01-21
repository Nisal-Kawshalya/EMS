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
    }
}
