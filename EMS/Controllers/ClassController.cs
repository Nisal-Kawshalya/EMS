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

        return RedirectToAction("Dashboard", "Teacher");
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
}
