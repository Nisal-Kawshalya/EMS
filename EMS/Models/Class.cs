using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models
{
    public class Class
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int TeacherId { get; set; }

        [Range(1, 13, ErrorMessage = "Grade must be between 1 and 13")]
        public required string Grade { get; set; }

        [Required(ErrorMessage = "Place can not  be empty")]
        [StringLength(100, ErrorMessage = "Place can not exceed 100 characters")]
        public required string Place { get; set; }

        [ForeignKey("TeacherId")]
        public Teacher? Teacher { get; set; }


        public ICollection<ClassStudent>? ClassStudents { get; set; } = new List<ClassStudent>();

        public ICollection<Result>? Results { get; set; } = new List<Result>();
        public ICollection<Attendence>? Attendences { get; set; } = new List<Attendence>();

        public ICollection<Note>? Notes { get; set; } = new List<Note>();
        public ICollection<Homework>? Homeworks { get; set; } = new List<Homework>();






    }
}
