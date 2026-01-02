using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

public class Note
{
    [Key]
    public int Id { get; set; }

    public int ClassId { get; set; }
    public int TeacherId { get; set; }

    public string Title { get; set; }
    public string NotesFilePath { get; set; }

    public DateTime CreatedAt { get; set; }

}
