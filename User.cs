namespace EventManagementAPI.Models
{
    public enum UserRole { Organiser, Participant, Admin } // Admin kept for backward compat with Part1 plan

    public class Users
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Hashed, never plain
        public UserRole Role { get; set; }

        public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
    }

    // DTOs
    public record RegisterDto(string FirstName, string LastName, string Email, string Password, string Role);
    public record LoginDto(string Email, string Password);
    public record UpdateProfileDto(string FirstName, string LastName);
}
