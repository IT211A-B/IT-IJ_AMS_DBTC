namespace AMS_Backend.Models
{
    public class Course
    {
        public Guid CourseId { get; set; } = Guid.NewGuid();

        public string CourseName { get; set; }

        public string Description { get; set; }
    }
}
