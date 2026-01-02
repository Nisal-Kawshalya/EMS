using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMS.Data;
using EMS.Models;

public class StudentsController : Controller
{
    private readonly ApplicationDbContext _context;

    public StudentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 👨‍🎓 SHOW STUDENTS TAB
    public IActionResult Index(int classId)
    {
        var cls = _context.Classes
            .Include(c => c.ClassStudents)
                .ThenInclude(cs => cs.Student)
            .FirstOrDefault(c => c.Id == classId);

        if (cls == null)
            return NotFound();

        ViewBag.Class = cls;
        return View(cls.ClassStudents.ToList());
    }

    // ➕ ADD STUDENT BY STUDENT CODE
    [HttpPost]
    public IActionResult Add(int classId, string studentCode)
    {
        var student = _context.Students
            .FirstOrDefault(s => s.StudentCode == studentCode);

        if (student == null)
        {
            TempData["Error"] = "Student not found";
            return RedirectToAction("Index", new { classId });
        }

        bool exists = _context.ClassStudents
            .Any(cs => cs.ClassId == classId && cs.StudentId == student.Id);

        if (!exists)
        {
            _context.ClassStudents.Add(new ClassStudent
            {
                ClassId = classId,
                StudentId = student.Id
            });
            _context.SaveChanges();
        }

        return RedirectToAction("Index", new { classId });
    }

    // 🗑 REMOVE STUDENT FROM CLASS
    public IActionResult Remove(int id, int classId)
    {
        var cs = _context.ClassStudents.Find(id);
        if (cs != null)
        {
            _context.ClassStudents.Remove(cs);
            _context.SaveChanges();
        }

        return RedirectToAction("Index", new { classId });
    }
}
