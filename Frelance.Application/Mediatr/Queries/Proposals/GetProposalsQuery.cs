using Frelance.Contracts.Dtos;
using Frelance.Contracts.Requests.Common;
using Frelance.Contracts.Responses.Common;
using MediatR;

namespace Frelance.Application.Mediatr.Queries.Proposals;

public record GetProposalsQuery(PaginationParams PaginationParams) : IRequest<PaginatedList<ProposalsDto>>;