namespace Frelance.Contracts.Dtos;

public record ReviewsDto(int Id, int ReviewerId, string ReviewText, DateTime CreatedAt, DateTime? UpdatedAt);