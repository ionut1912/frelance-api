namespace Frelance.Infrastructure.Entities;

public class ProjectTechnologies
{
    public int Id { get; set; }
    public string Technology { get; set; }
    public int ProjectId { get; set; }
    public Projects Projects { get; set; }
}