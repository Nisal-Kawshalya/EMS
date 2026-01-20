using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMS.Data;
using EMS.Models;

public class ResultsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ResultsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 📊 SHOW RESULTS TAB
    public IActionResult Index(int classId)
    {
        var cls = _context.Classes
            .Include(c => c.Results)
                .ThenInclude(r => r.Student)
            .FirstOrDefault(c => c.Id == classId);

        if (cls == null)
            return NotFound();

        ViewBag.Class = cls;
        return View(cls.Results.ToList());
    }

    // ➕ ENTER RESULTS PAGE
    public IActionResult Create(int classId)
    {
        var students = _context.ClassStudents
            .Where(cs => cs.ClassId == classId)
            .Include(cs => cs.Student)
            .Select(cs => cs.Student)
            .ToList();

        ViewBag.ClassId = classId;
        return View(students);
    }

    // 💾 SAVE RESULTS
    [HttpPost]
    public IActionResult Create(int classId, string examTitle, Dictionary<int, int> marks)
    {
        foreach (var item in marks)
        {
            _context.Results.Add(new Result
            {
                ClassId = classId,
                StudentId = item.Key,
                ExamTitle = examTitle,
                Marks = item.Value
            });
        }

        _context.SaveChanges();
        return RedirectToAction("Index", new { classId });
    }
}
