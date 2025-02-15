using Frelance.Application.Helpers;
using Frelance.Application.Mediatr.Commands.ClientProfiles;
using Frelance.Application.Mediatr.Queries.ClientProfiles;
using Frelance.Application.Repositories;
using Frelance.Application.Repositories.External;
using Frelance.Contracts.Dtos;
using Frelance.Contracts.Enums;
using Frelance.Contracts.Exceptions;
using Frelance.Contracts.Responses.Common;
using Frelance.Infrastructure.Context;
using Frelance.Infrastructure.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Frelance.Infrastructure.Services;

public class ClientProfileRepository : IClientProfileRepository
{
    private readonly FrelanceDbContext _dbContext;
    private readonly IBlobService _blobService;
    private readonly IUserAccessor _userAccessor;

    public ClientProfileRepository(FrelanceDbContext dbContext, IBlobService blobService, IUserAccessor userAccessor)
    {
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(blobService, nameof(blobService));
        ArgumentNullException.ThrowIfNull(userAccessor, nameof(userAccessor));
        _dbContext = dbContext;
        _blobService = blobService;
        _userAccessor = userAccessor;
    }

    public async Task AddClientProfileAsync(CreateClientProfileCommand clientProfileCommand, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserName == _userAccessor.GetUsername(), cancellationToken);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }
        var clientProfile = clientProfileCommand.Adapt<ClientProfiles>();
        await _dbContext.Addresses.AddAsync(clientProfile.Addresses, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        clientProfile.UserId = user.Id;
        clientProfile.AddressId = clientProfile.Addresses.Id;
        var profileImageUrl = await _blobService.UploadBlobAsync(StorageContainers.USERIMAGESCONTAINER.ToString().ToLower(),
            $"{user.Id}/{clientProfileCommand.CreateClientProfileRequest.ProfileImage.FileName}", clientProfileCommand.CreateClientProfileRequest.ProfileImage);
        clientProfile.ProfileImageUrl = profileImageUrl;
        clientProfile.Bio = clientProfileCommand.CreateClientProfileRequest.Bio;
        await _dbContext.ClientProfiles.AddAsync(clientProfile, cancellationToken);
    }

    public async Task<ClientProfileDto> GetClientProfileByIdAsync(GetClientProfileByIdQuery query, CancellationToken cancellationToken)
    {
        var clientProfile = await _dbContext.Set<ClientProfiles>()
            .AsNoTracking()
            .Include(cp => cp.Users)
            .ThenInclude(u => u.Reviews)
            .Include(cp => cp.Users)
            .ThenInclude(u => u.Proposals)
            .Include(cp => cp.Contracts)
            .Include(cp => cp.Invoices)
            .Include(x => x.Projects)
            .Include(x=>x.Addresses)
            .FirstOrDefaultAsync(cp => cp.Id == query.Id, cancellationToken);


        if (clientProfile is null)
        {
            throw new NotFoundException($"{nameof(ClientProfiles)} with {nameof(ClientProfiles.Id)} : '{query.Id}' does not exist");
        }

        return clientProfile.Adapt<ClientProfileDto>();
    }

    public async Task<PaginatedList<ClientProfileDto>> GetClientProfilesAsync(GetClientProfilesQuery clientProfilesQuery, CancellationToken cancellationToken)
    {
        var clientsQuery = _dbContext.ClientProfiles
            .AsNoTracking()
            .Include(x => x.Users)
            .ThenInclude(x => x.Reviews)
            .Include(x => x.Users)
            .ThenInclude(x => x.Proposals)
            .Include(x => x.Addresses)
            .Include(x => x.Contracts)
            .Include(x => x.Invoices)
            .Include(x => x.Projects)
            .ProjectToType<ClientProfileDto>();

        var count = await clientsQuery.CountAsync(cancellationToken);
        var items = await clientsQuery
            .Skip((clientProfilesQuery.PaginationParams.PageNumber - 1) * clientProfilesQuery.PaginationParams.PageSize)
            .Take(clientProfilesQuery.PaginationParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<ClientProfileDto>(items, count, clientProfilesQuery.PaginationParams.PageNumber, clientProfilesQuery.PaginationParams.PageSize);
    }

    public async Task UpdateClientProfileAsync(UpdateClientProfileCommand clientProfileCommand, CancellationToken cancellationToken)
    {
        var clientToUpdate = await _dbContext.ClientProfiles
                                                         .AsNoTracking()
                                                         .Include(x => x.Addresses)
                                                         .FirstOrDefaultAsync(x => x.Id == clientProfileCommand.Id, cancellationToken);

        if (clientToUpdate is null)
        {
            throw new NotFoundException($"{nameof(ClientProfiles)} with {nameof(ClientProfiles.Id)} : '{clientProfileCommand.Id}' does not exist");
        }
        clientToUpdate = clientProfileCommand.Adapt<ClientProfiles>();
        if (clientProfileCommand.UpdateClientProfileRequest.ProfileImage is not null)
        {
            await _blobService.DeleteBlobAsync(StorageContainers.USERIMAGESCONTAINER.ToString().ToLower(), clientToUpdate.UserId.ToString());
            clientToUpdate.ProfileImageUrl = await _blobService.UploadBlobAsync(StorageContainers.USERIMAGESCONTAINER.ToString().ToLower(),
                $"{clientToUpdate.UserId}/{clientProfileCommand.UpdateClientProfileRequest.ProfileImage.FileName}",
                clientProfileCommand.UpdateClientProfileRequest.ProfileImage);
        }

        _dbContext.ClientProfiles.Update(clientToUpdate);
    }

    public async Task DeleteClientProfileAsync(DeleteClientProfileCommand clientProfileCommand, CancellationToken cancellationToken)
    {
        var clientToDelete = await _dbContext.ClientProfiles
                                                        .AsNoTracking()
                                                        .FirstOrDefaultAsync(x => x.Id == clientProfileCommand.Id, cancellationToken);
        if (clientToDelete is null)
        {
            throw new NotFoundException($"{nameof(ClientProfiles)} with {nameof(ClientProfiles.Id)} : '{clientProfileCommand.Id}' does not exist");
        }

        await _blobService.DeleteBlobAsync(StorageContainers.USERIMAGESCONTAINER.ToString().ToLower(), clientToDelete.UserId.ToString());
        _dbContext.ClientProfiles.Remove(clientToDelete);
    }
}