namespace EventManagementAPI.Models
{
    // This is your "Participate" entity from Part 1 plan
    public class Enrolments
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Participant
        public User? User { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public string Status { get; set; } = "Registered";
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

        public ICollection<Result> Results { get; set; } = new List<Result>();
    }

    public record CreateEnrolmentDto(int EventId, int? CategoryId);
    public record UpdateEnrolmentStatusDto(string Status);
}
