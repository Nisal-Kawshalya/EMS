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

    // ✅ DELETE CLASS (with confirmation modal)
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

}
