namespace EventManagementAPI.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // event_name
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty; // required by POE
        public double Distance { get; set; } // e.g. 10, 21
        public string EventType { get; set; } = "run"; // run, walk, or cycle
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? VenueId { get; set; }
        public Venue? Venue { get; set; }
        public int CreatedByUserId { get; set; }

        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }

    public record CreateEventDto(string Name, string Description, DateTime EventDate, string Location, double Distance, string EventType, int? CategoryId, int? VenueId);
    public record UpdateEventDto(string Name, string Description, DateTime EventDate, string Location, double Distance, string EventType, int? CategoryId, int? VenueId);
}
