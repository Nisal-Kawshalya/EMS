using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models
{
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }
        public required string StudentCode { get; set; }

        [Required(ErrorMessage = "Name can not be empty")]
        [StringLength(100, MinimumLength = 5)]
        [Display(Name = "Enter Full Name")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "PhoneNumber can not be empty")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        [Display(Name = "Phone Number")]
        public required string PhoneNumber { get; set; }


        [ForeignKey("UserId")]
        public User? User { get; set; }

    }
}
