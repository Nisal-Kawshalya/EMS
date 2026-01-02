using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace EMS.Models
{
    public class Note
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClassId { get; set; }
        public int TeacherId { get; set; }

        [Required(ErrorMessage = "Title cannot be empty")]
        [StringLength(100, ErrorMessage = "Title can not exceed 100 characters")]
        [Display(Name = "Note Title")]
        public required string Title { get; set; }

        [Display(Name = "Notes PDF")]
        public string? NotesFile { get; set; }


        [Required]
        public DateTime Createddate { get; set; }


        [ForeignKey("ClassId")]
        public Class? Class { get; set; }

    }
}
