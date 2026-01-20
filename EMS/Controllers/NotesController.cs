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

    // ✅ SHOW NOTES TAB
    public IActionResult Index(int classId, int showForm = 0)
    {
        var cls = _context.Classes
            .Include(c => c.Notes)
            .FirstOrDefault(c => c.Id == classId);

        if (cls == null) return NotFound();

        ViewBag.Class = cls;
        ViewBag.ShowForm = showForm == 1;

        return View(cls.Notes.ToList());
    }

    // ✅ If user goes to /Notes/Create -> redirect to Index + open form
    [HttpGet]
    public IActionResult Create(int classId)
    {
        return RedirectToAction("Index", new { classId, showForm = 1 });
    }

    // ✅ SAVE NOTES (PDF)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Note note, IFormFile pdfFile)
    {
        if (note == null || note.ClassId <= 0)
            return BadRequest();

        if (string.IsNullOrWhiteSpace(note.Title))
        {
            TempData["NotesError"] = "Title is required.";
            return RedirectToAction("Index", new { classId = note.ClassId, showForm = 1 });
        }

        if (pdfFile == null || pdfFile.Length == 0)
        {
            TempData["NotesError"] = "Please upload a PDF file.";
            return RedirectToAction("Index", new { classId = note.ClassId, showForm = 1 });
        }

        var ext = Path.GetExtension(pdfFile.FileName).ToLower();
        if (ext != ".pdf")
        {
            TempData["NotesError"] = "Only PDF files are allowed.";
            return RedirectToAction("Index", new { classId = note.ClassId, showForm = 1 });
        }

        string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "notes");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        string fileName = Guid.NewGuid() + ext;
        string filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            pdfFile.CopyTo(stream);
        }

        note.NotesFile = "/uploads/notes/" + fileName;
        note.Createddate = DateTime.Now;

        _context.Notes.Add(note);
        _context.SaveChanges();

        return RedirectToAction("Index", new { classId = note.ClassId });
    }

    // ✅ DELETE NOTE
    public IActionResult Delete(int id)
    {
        var note = _context.Notes.Find(id);
        if (note == null) return NotFound();

        int classId = note.ClassId;

        if (!string.IsNullOrEmpty(note.NotesFile))
        {
            var physicalPath = Path.Combine(
                _env.WebRootPath,
                note.NotesFile.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString())
            );

            if (System.IO.File.Exists(physicalPath))
                System.IO.File.Delete(physicalPath);
        }

        _context.Notes.Remove(note);
        _context.SaveChanges();

        return RedirectToAction("Index", new { classId });
    }
}
