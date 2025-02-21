using Frelance.Contracts.Dtos;
using Frelance.Contracts.Requests.Common;
using Frelance.Contracts.Responses.Common;
using MediatR;

namespace Frelance.Application.Mediatr.Queries.FreelancerProfiles;

public record GetFreelancerProfilesQuery(PaginationParams PaginationParams)
    : IRequest<PaginatedList<FreelancerProfileDto>>;