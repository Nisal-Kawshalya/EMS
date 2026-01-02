using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Models
{
    public class ClassStudent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int StudentId { get; set; }


        [ForeignKey("ClassId")]
        public Class? Class { get; set; }


    }
}
