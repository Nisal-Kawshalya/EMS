using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMS.Data;
using EMS.Models;

public class AttendanceController : Controller
{
    private readonly ApplicationDbContext _context;

    public AttendanceController(ApplicationDbContext context)
    {
        _context = context;
    }

    // Show attendance page (with stats)
    [HttpGet]
    public IActionResult Index(int classId, DateTime? date)
    {
        var cls = _context.Classes
            .Include(c => c.ClassStudents)
                .ThenInclude(cs => cs.Student)
            .FirstOrDefault(c => c.Id == classId);

        if (cls == null) return NotFound();

        var selectedDate = (date ?? DateTime.Today).Date;

        // Existing attendance for that date (if any)
        var records = _context.Attendences
            .Where(a => a.ClassId == classId && a.Date == selectedDate)
            .ToList();

        // Students of class
        var students = cls.ClassStudents
            .Select(cs => cs.Student)
            .Where(s => s != null)
            .ToList();

        // Build a map: StudentId -> IsPresent
        var statusMap = records.ToDictionary(r => r.StudentId, r => r.IsPresent);

        int total = students.Count;
        int presentCount = records.Count(r => r.IsPresent);
        int absentCount = total - presentCount;
        int percent = total == 0 ? 0 : (int)Math.Round((presentCount * 100.0) / total);

        ViewBag.Class = cls;
        ViewBag.SelectedDate = selectedDate;
        ViewBag.Present = presentCount;
        ViewBag.Absent = absentCount;
        ViewBag.Percent = percent;
        ViewBag.StatusMap = statusMap;

        return View(students);
    }

    // Save attendance (same as your logic, but redirects back to Index with selected date)
    [HttpPost]
    public IActionResult Save(int classId, DateTime date, List<int> presentStudentIds)
    {
        date = date.Date;

        var old = _context.Attendences
            .Where(a => a.ClassId == classId && a.Date == date)
            .ToList();

        _context.Attendences.RemoveRange(old);

        var students = _context.ClassStudents
            .Where(cs => cs.ClassId == classId)
            .Select(cs => cs.StudentId)
            .ToList();

        foreach (var studentId in students)
        {
            _context.Attendences.Add(new Attendence
            {
                ClassId = classId,
                StudentId = studentId,
                Date = date,
                IsPresent = presentStudentIds.Contains(studentId)
            });
        }

        _context.SaveChanges();

        return RedirectToAction("Index", new { classId = classId, date = date.ToString("yyyy-MM-dd") });
    }

    // Download report for selected day (simple CSV)
    [HttpGet]
    public IActionResult Report(int classId, DateTime date)
    {
        date = date.Date;

        var cls = _context.Classes.FirstOrDefault(c => c.Id == classId);
        if (cls == null) return NotFound();

        var rows = _context.Attendences
            .Where(a => a.ClassId == classId && a.Date == date)
            .Join(_context.Students,
                a => a.StudentId,
                s => s.Id,
                (a, s) => new
                {
                    s.StudentCode,
                    s.Name,
                    Status = a.IsPresent ? "Present" : "Absent"
                })
            .ToList();

        // CSV content
        var csv = "StudentCode,Name,Status\n";
        foreach (var r in rows)
        {
            csv += $"{Escape(r.StudentCode)},{Escape(r.Name)},{r.Status}\n";
        }

        var fileName = $"attendance_class_{classId}_{date:yyyy-MM-dd}.csv";
        return File(System.Text.Encoding.UTF8.GetBytes(csv), "text/csv", fileName);
    }

    private string Escape(string? text)
    {
        text ??= "";
        if (text.Contains(",") || text.Contains("\""))
        {
            text = text.Replace("\"", "\"\"");
            return $"\"{text}\"";
        }
        return text;
    }
}
