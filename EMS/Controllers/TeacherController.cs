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

        var classes = _context.Classes
            .Where(c => c.TeacherId == teacher!.Id)
            .Include(c => c.ClassStudents)
            .ToList();

        return View(classes);
    }
}
