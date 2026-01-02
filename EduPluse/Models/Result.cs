using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models
{
    public class Result
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int ClassId { get; set; }

        [Required(ErrorMessage = "ExamTitle can not be empty")]
        [StringLength(100, ErrorMessage = "ExamTitle can not exceed 100 characters")]
        [Display(Name = "Exam Title")]
        public required string ExamTitle { get; set; }

        [Required]
        [Range(0, 100, ErrorMessage = "Marks must be between 1 and 13")]
        public int Marks { get; set; }



        [ForeignKey("ClassId")]
        public Class? Class { get; set; }



    }
}
