namespace EduPluse.Models
{
    public class Homework
    {
        public int id { get; set; }
        public int ClassId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AssssignmentFilePath { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
