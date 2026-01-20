using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMS.Data;
using EMS.Models;

public class AddstudentController : Controller
{
    private readonly ApplicationDbContext _context;

    public AddstudentController(ApplicationDbContext context)
    {
        _context = context;
    }

    // ✅ Students tab page (Teacher)
    public IActionResult Index(int classId)
    {
        var cls = _context.Classes
            .Include(c => c.ClassStudents)
                .ThenInclude(cs => cs.Student)
            .FirstOrDefault(c => c.Id == classId);

        if (cls == null) return NotFound();

        ViewBag.Class = cls;

        // For inline form open: ?showForm=1
        ViewBag.ShowForm = Request.Query["showForm"] == "1";

        return View(cls.ClassStudents.ToList());
    }

    // ✅ If someone goes /Addstudent/Create, redirect to Index + open form
    [HttpGet]
    public IActionResult Create(int classId)
    {
        return RedirectToAction("Index", new { classId, showForm = 1 });
    }

    // ✅ ADD student to class by StudentCode (recommended)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(int classId, string studentCode)
    {
        if (classId <= 0) return BadRequest();

        if (string.IsNullOrWhiteSpace(studentCode))
        {
            TempData["StudentError"] = "Student Code is required.";
            return RedirectToAction("Index", new { classId, showForm = 1 });
        }

        studentCode = studentCode.Trim();

        var student = _context.Students.FirstOrDefault(s => s.StudentCode == studentCode);
        if (student == null)
        {
            TempData["StudentError"] = "Student not found. Check Student Code.";
            return RedirectToAction("Index", new { classId, showForm = 1 });
        }

        bool already = _context.ClassStudents.Any(cs => cs.ClassId == classId && cs.StudentId == student.Id);
        if (already)
        {
            TempData["StudentError"] = "This student is already in this class.";
            return RedirectToAction("Index", new { classId, showForm = 1 });
        }

        _context.ClassStudents.Add(new ClassStudent
        {
            ClassId = classId,
            StudentId = student.Id
        });

        _context.SaveChanges();

        return RedirectToAction("Index", new { classId });
    }

    // ✅ REMOVE student from class
    public IActionResult Remove(int id)
    {
        var cs = _context.ClassStudents
            .Include(x => x.Class)
            .FirstOrDefault(x => x.Id == id);

        if (cs == null) return NotFound();

        int classId = cs.ClassId;

        _context.ClassStudents.Remove(cs);
        _context.SaveChanges();

        return RedirectToAction("Index", new { classId });
    }
}
