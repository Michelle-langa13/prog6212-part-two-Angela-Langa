namespace EventManagementAPI.Models
{
    public class Result
    {
        public int Id { get; set; }
        public int EnrolmentId { get; set; }
        public Enrolment? Enrolment { get; set; }
        public TimeSpan FinishTime { get; set; }
        public int Position { get; set; }
        public int PointsEarned { get; set; }
    }

    public record CreateResultDto(int EnrolmentId, string FinishTime, int Position, int PointsEarned);
    public record UpdateResultDto(string FinishTime, int Position, int PointsEarned);
}