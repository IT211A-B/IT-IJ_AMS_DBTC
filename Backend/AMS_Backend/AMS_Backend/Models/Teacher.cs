namespace AMS_Backend.Models
{
    public class Teacher
    {
        public Guid TeacherId { get; set; } = Guid.NewGuid();

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
    }
}
