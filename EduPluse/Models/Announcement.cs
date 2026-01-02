using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models
{
    public class Announcement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Title cannot be empty")]
        [StringLength(100, ErrorMessage = "Title can not exceed 100 characters")]
        [Display(Name = "Announcement Title")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Description can not be empty")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        [Display(Name = "Announcement Description")]
        public required string Description { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Announcement Date and Time")]

        public DateTime DateTime { get; set; }



        [ForeignKey("ClassId")]
        public Class? Class { get; set; }




    }
}
