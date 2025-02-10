namespace Frelance.Infrastructure.Entities;

public class BaseUserProfile
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required Users Users { get; set; }
    public int AddressId { get; set; }
    public required Addresses Addresses { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImageUrl { get; set; }
    public List<Contracts> Contracts { get; set; } = [];
    public List<Invoices> Invoices { get; set; } = [];
}