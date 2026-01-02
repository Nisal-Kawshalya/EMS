using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EMS.Data;
using EMS.Models;

public class NotesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _env;

    public NotesController(ApplicationDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }

    // 📘 SHOW NOTES TAB
    public IActionResult Index(int classId)
    {
        var cls = _context.Classes
            .Include(c => c.Notes)
            .FirstOrDefault(c => c.Id == classId);

        if (cls == null)
            return NotFound();

        ViewBag.Class = cls;
        return View(cls.Notes.ToList());
    }

    // ➕ ADD NOTES PAGE
    public IActionResult Create(int classId)
    {
        ViewBag.ClassId = classId;
        return View();
    }

    // 💾 SAVE NOTES (PDF)
    [HttpPost]
    public IActionResult Create(Note note, IFormFile pdfFile)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.ClassId = note.ClassId;
            return View(note);
        }

        if (pdfFile != null && pdfFile.Length > 0)
        {
            // 🔑 Correct & safe upload path
            string uploadsFolder = Path.Combine(
                _env.WebRootPath,
                "uploads",
                "notes"
            );

            // 🔑 CREATE FOLDER IF NOT EXISTS
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string fileName = Guid.NewGuid() + Path.GetExtension(pdfFile.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                pdfFile.CopyTo(stream);
            }

            note.NotesFile = "/uploads/notes/" + fileName;
        }

        note.Createddate = DateTime.Now;

        _context.Notes.Add(note);
        _context.SaveChanges();

        return RedirectToAction("Index", new { classId = note.ClassId });
    }

    // 🗑 DELETE NOTE
    public IActionResult Delete(int id)
    {
        var note = _context.Notes.Find(id);
        if (note == null)
            return NotFound();

        int classId = note.ClassId;

        _context.Notes.Remove(note);
        _context.SaveChanges();

        return RedirectToAction("Index", new { classId });
    }
}
