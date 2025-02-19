using Frelance.Application.Mediatr.Commands.ClientProfiles;
using Frelance.Application.Mediatr.Queries.ClientProfiles;
using Frelance.Contracts.Dtos;
using Frelance.Contracts.Responses.Common;

namespace Frelance.Application.Repositories;

public interface IClientProfileRepository
{
    Task AddClientProfileAsync(CreateClientProfileCommand clientProfileDto, CancellationToken cancellationToken);
    Task<ClientProfileDto> GetClientProfileByIdAsync(GetClientProfileByIdQuery clientProfileByIdQuery, CancellationToken cancellationToken);
    Task<ClientProfileDto?> GetLoggedInClientProfileAsync(GetLoggedInClientProfileQuery loggedInClientProfileQuery, CancellationToken cancellationToken);
    Task<PaginatedList<ClientProfileDto>> GetClientProfilesAsync(GetClientProfilesQuery clientProfilesQuery, CancellationToken cancellationToken);
    Task UpdateClientProfileAsync(UpdateClientProfileCommand clientProfileCommand, CancellationToken cancellationToken);
    Task DeleteClientProfileAsync(DeleteClientProfileCommand clientProfileCommand, CancellationToken cancellationToken);
}