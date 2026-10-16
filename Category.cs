namespace EventManagementAPI.Models
{
    public class Categories
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g. Under 20, Senior, 10km, 21km
        public string AgeGroup { get; set; } = string.Empty;
        public string Distance { get; set; } = string.Empty;
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
    public record CreateCategoryDto(string Name, string AgeGroup, string Distance);
}
