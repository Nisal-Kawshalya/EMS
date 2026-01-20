using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMS.Data;

public class TeacherController : Controller
{
    private readonly ApplicationDbContext _context;

    public TeacherController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Dashboard()
    {
        int userId = HttpContext.Session.GetInt32("UserId")!.Value;

        var teacher = _context.Teachers
            .FirstOrDefault(t => t.UserId == userId);

        if (teacher == null)
            return RedirectToAction("Login", "Account");

        //  ALL classes for totals
        var allClasses = _context.Classes
            .Where(c => c.TeacherId == teacher.Id)
            .Include(c => c.ClassStudents)
            .ToList();

        //  ONLY 4 recent classes to show in dashboard list
        var recentClasses = allClasses
            .OrderByDescending(c => c.Id)
            .Take(4)
            .ToList();

        //  totals (use allClasses, not recentClasses)
        ViewBag.TotalClasses = allClasses.Count;
        ViewBag.TotalStudents = allClasses.Sum(c => c.ClassStudents.Count);

        return View(recentClasses);
    }
}
