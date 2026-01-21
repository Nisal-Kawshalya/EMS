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

    // ✅ SHOW RESULTS TAB
    public IActionResult Index(int classId, int showForm = 0)
    {
        var cls = _context.Classes
            .Include(c => c.ClassStudents)
                .ThenInclude(cs => cs.Student)
            .Include(c => c.Results)
                .ThenInclude(r => r.Student)
            .FirstOrDefault(c => c.Id == classId);

        if (cls == null)
            return NotFound();

        ViewBag.Class = cls;
        ViewBag.ClassId = classId;
        ViewBag.ShowForm = showForm == 1;

        // students for the inline form
        ViewBag.Students = cls.ClassStudents
            .Where(cs => cs.Student != null)
            .Select(cs => cs.Student)
            .ToList();

        return View(cls.Results.ToList());
    }

    // ✅ If someone visits /Results/Create, redirect to Index and open the form
    [HttpGet]
    public IActionResult Create(int classId)
    {
        if (classId <= 0) return BadRequest();
        return RedirectToAction("Index", new { classId, showForm = 1 });
    }

    // ✅ SAVE RESULTS (INLINE FORM POST)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(int classId, string examTitle, DateTime examDate, Dictionary<int, int> marks)
    {
        if (classId <= 0) return BadRequest();

        if (string.IsNullOrWhiteSpace(examTitle))
        {
            TempData["ResultsError"] = "Exam title is required.";
            return RedirectToAction("Index", new { classId, showForm = 1 });
        }

        if (marks == null || marks.Count == 0)
        {
            TempData["ResultsError"] = "Please enter marks.";
            return RedirectToAction("Index", new { classId, showForm = 1 });
        }

        // ✅ Optional: prevent duplicates for same exam (same title + date)
        var existing = _context.Results
            .Where(r => r.ClassId == classId && r.ExamTitle == examTitle && r.ExamDate == examDate)
            .ToList();

        if (existing.Count > 0)
        {
            _context.Results.RemoveRange(existing);
            _context.SaveChanges();
        }

        foreach (var item in marks)
        {
            var studentId = item.Key;
            var score = item.Value;

            if (score < 0) score = 0;
            if (score > 100) score = 100;

            _context.Results.Add(new Result
            {
                ClassId = classId,
                StudentId = studentId,
                ExamTitle = examTitle.Trim(),
                ExamDate = examDate,
                Marks = score
            });
        }

        _context.SaveChanges();
        return RedirectToAction("Index", new { classId });
    }

    // ✅ STUDENT VIEW: show only logged-in student's results for a class
    [HttpGet]
    public IActionResult Student(int classId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var role = HttpContext.Session.GetString("Role");

        if (userId == null || role != "Student")
            return RedirectToAction("Login", "Account");

        var student = _context.Students.FirstOrDefault(s => s.UserId == userId.Value);
        if (student == null) return RedirectToAction("Login", "Account");

        var enrolled = _context.ClassStudents.Any(cs => cs.ClassId == classId && cs.StudentId == student.Id);
        if (!enrolled) return Forbid();

        var cls = _context.Classes
            .Include(c => c.Teacher)
            .FirstOrDefault(c => c.Id == classId);

        if (cls == null) return NotFound();

        var list = _context.Results
            .Where(r => r.ClassId == classId && r.StudentId == student.Id)
            .OrderByDescending(r => r.ExamDate)
            .ToList();

        ViewBag.Class = cls;
        ViewBag.Student = student;

        return View("Student", list);
    }
}
