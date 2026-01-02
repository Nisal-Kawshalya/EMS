using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models
{
    public class Homework
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Title cannot be empty")]
        [StringLength(100, ErrorMessage = "Title can not exceed 100 characters")]
        [Display(Name = "Homeworke Title")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Description can not be empty")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Announcement Description")]
        public required string Description { get; set; }

        [Display(Name = "Assignment PDF")]
        public string? AssssignmentFile { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Due Date")]
        public DateTime DueDate { get; set; }

        [Required]
        public DateTime CreatedDate { get; set; }

        [ForeignKey("ClassId")]
        public Class? Class { get; set; }

    }
}
