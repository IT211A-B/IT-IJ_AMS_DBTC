namespace AMS_Backend.Models
{
    public class Student
    {
        public Guid StudentId { get; set; } =  Guid.NewGuid();

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}