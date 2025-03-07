namespace Frelance.Infrastructure.Entities;

public class BaseUserProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public Users? Users { get; set; }
    public int AddressId { get; set; }
    public Addresses? Addresses { get; set; }
    public required string Bio { get; set; }
    public List<Contracts> Contracts { get; set; } = [];
    public List<Invoices> Invoices { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<Projects>? Projects { get; set; } = [];
    public required string Image { get; set; }
    public bool IsVerified { get; set; }
}