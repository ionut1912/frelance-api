namespace Frelance.Infrastructure.Entities;

public class TimeLogs
{
    public int Id { get; set; }
    public int TaskId { get; set; }
    public required ProjectTasks Task { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateOnly Date { get; set; }
    public int FreelancerProfileId { get; set; }
    public required FreelancerProfiles FreelancerProfiles { get; set; }
    public int TotalHours { get; set; }
}