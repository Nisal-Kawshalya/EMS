using Microsoft.AspNetCore.Mvc;
using EMS.Data;
using EMS.Models;

public class AttendanceController : Controller
{
    private readonly ApplicationDbContext _context;

    public AttendanceController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public IActionResult Save(int classId, DateTime date, List<int> presentStudentIds)
    {
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
        return RedirectToAction("Details", "Class", new { id = classId });
    }
}
