namespace AMS_Backend.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }

        public int StudentId { get; set; }

        public int TeacherId { get; set; }

        public DateTime Date { get; set; }

        public string Status { get; set; }
    }
}
