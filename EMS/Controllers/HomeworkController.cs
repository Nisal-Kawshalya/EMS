using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMS.Data;
using EMS.Models;

public class HomeworkController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public HomeworkController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // 📘 SHOW HOMEWORK TAB
    public IActionResult Index(int classId)
    {
        var cls = _context.Classes
            .Include(c => c.Homeworks)
            .FirstOrDefault(c => c.Id == classId);

        if (cls == null)
            return NotFound();

        ViewBag.Class = cls;
        return View(cls.Homeworks.ToList());
    }

    // ➕ ADD HOMEWORK PAGE
    public IActionResult Create(int classId)
    {
        ViewBag.ClassId = classId;
        return View();
    }

    // 💾 SAVE HOMEWORK (PDF)
    [HttpPost]
    public IActionResult Create(Homework homework, IFormFile pdfFile)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ClassId = homework.ClassId;
            return View(homework);
        }

        // PDF upload
        if (pdfFile != null && pdfFile.Length > 0)
        {
            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads/homework");
            string fileName = Guid.NewGuid() + Path.GetExtension(pdfFile.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            pdfFile.CopyTo(stream);

            homework.AssssignmentFile = "/uploads/homework/" + fileName;
        }

        homework.CreatedDate = DateTime.Now;

        _context.Homeworks.Add(homework);
        _context.SaveChanges();

        return RedirectToAction("Index", new { classId = homework.ClassId });
    }

    // 🗑 DELETE HOMEWORK
    public IActionResult Delete(int id)
    {
        var hw = _context.Homeworks.Find(id);
        if (hw == null)
            return NotFound();

        int classId = hw.ClassId;

        _context.Homeworks.Remove(hw);
        _context.SaveChanges();

        return RedirectToAction("Index", new { classId });
    }
}
