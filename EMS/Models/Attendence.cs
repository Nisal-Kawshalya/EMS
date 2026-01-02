using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models
{
    public class Attendence
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int ClassId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Attendance Date")]
        public DateTime Date { get; set; }

        [Required]
        public bool IsPresent { get; set; }


        [ForeignKey("ClassId")]
        public Class? Class { get; set; }


        [ForeignKey("StudentId")]
        public Student? Student { get; set; }
    }
}
