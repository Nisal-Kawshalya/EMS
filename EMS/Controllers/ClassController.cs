using EMS.Data;
using EMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class ClassController : Controller
{
    private readonly ApplicationDbContext _context;

    public ClassController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        int userId = HttpContext.Session.GetInt32("UserId")!.Value;
        var teacher = _context.Teachers.FirstOrDefault(t => t.UserId == userId);

        if (teacher == null) return RedirectToAction("Login", "Account");

        var classes = _context.Classes
            .Where(c => c.TeacherId == teacher.Id)
            .Include(c => c.ClassStudents)
            .ToList();

        return View(classes);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Class cls)
    {
        if (!ModelState.IsValid)
            return View(cls);

        int userId = HttpContext.Session.GetInt32("UserId")!.Value;
        var teacher = _context.Teachers.First(t => t.UserId == userId);

        cls.TeacherId = teacher.Id;

        _context.Classes.Add(cls);
        _context.SaveChanges();

        return RedirectToAction("Index", "Class");
    }

    public IActionResult Details(int id)
    {
        var cls = _context.Classes
            .Include(c => c.ClassStudents)
                .ThenInclude(cs => cs.Student)
            .FirstOrDefault(c => c.Id == id);

        ViewBag.ActiveTab = "Attendance";
        return View(cls);
    }

    // ✅ DELETE CLASS
    [HttpPost]
    public IActionResult Delete(int id)
    {
        int userId = HttpContext.Session.GetInt32("UserId")!.Value;
        var teacher = _context.Teachers.FirstOrDefault(t => t.UserId == userId);
        if (teacher == null) return RedirectToAction("Login", "Account");

        var cls = _context.Classes
            .Include(c => c.ClassStudents)
            .FirstOrDefault(c => c.Id == id && c.TeacherId == teacher.Id);

        if (cls == null) return NotFound();

        if (cls.ClassStudents != null && cls.ClassStudents.Count > 0)
            _context.ClassStudents.RemoveRange(cls.ClassStudents);

        _context.Classes.Remove(cls);
        _context.SaveChanges();

        return RedirectToAction("Index", "Class");
    }

    // ✅ ENROLL STUDENT BY STUDENT CODE
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult EnrollByCode(int classId, string studentCode)
    {
        int userId = HttpContext.Session.GetInt32("UserId")!.Value;
        var teacher = _context.Teachers.FirstOrDefault(t => t.UserId == userId);
        if (teacher == null) return RedirectToAction("Login", "Account");

        studentCode = (studentCode ?? "").Trim();

        var student = _context.Students.FirstOrDefault(s => s.StudentCode == studentCode);
        if (student == null)
        {
            TempData["Error"] = "Student code not found.";
            return RedirectToAction("Details", new { id = classId });
        }

        // ensure class belongs to this teacher
        var cls = _context.Classes.FirstOrDefault(c => c.Id == classId && c.TeacherId == teacher.Id);
        if (cls == null) return NotFound();

        // prevent duplicates
        bool already = _context.ClassStudents.Any(cs => cs.ClassId == classId && cs.StudentId == student.Id);
        if (!already)
        {
            _context.ClassStudents.Add(new ClassStudent
            {
                ClassId = classId,
                StudentId = student.Id
            });
            _context.SaveChanges();
            TempData["Success"] = $"Enrolled: {student.Name} ({student.StudentCode})";
        }
        else
        {
            TempData["Info"] = "This student is already enrolled.";
        }

        return RedirectToAction("Details", new { id = classId });
    }
}
