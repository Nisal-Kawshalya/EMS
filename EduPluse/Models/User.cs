using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public required string Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        [Required]
        public required string Role { get; set; }


    }
}
