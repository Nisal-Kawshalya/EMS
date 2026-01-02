using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace EMS.Models
{
    public class Teacher
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        public required string TeacherCode { get; set; }

        [Required(ErrorMessage = "Name can not empty")]
        [StringLength(100, MinimumLength = 5)]
        [Display(Name = "Enter  Your Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "PhoneNumber can not be empty")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Phone number must be exactly 10 digits")]
        [Display(Name = "Phone Number")]
        public required string PhoneNumber { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        public ICollection<Class>? Classes { get; set; } = new List<Class>();

    }
}
