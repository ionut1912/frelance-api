using Frelance.Application.Mediatr.Commands.Contracts;
using Frelance.Application.Mediatr.Queries.Contracts;
using Frelance.Application.Repositories;
using Frelance.Contracts.Dtos;
using Frelance.Contracts.Exceptions;
using Frelance.Contracts.Responses.Common;
using Frelance.Infrastructure.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Frelance.Infrastructure.Services;

public class ContractRepository : IContractRepository
{
    private readonly IGenericRepository<ClientProfiles> _clientProfilesRepository;
    private readonly IGenericRepository<Entities.Contracts> _contractsRepository;
    private readonly IGenericRepository<FreelancerProfiles> _freelancerProfilesRepository;
    private readonly IGenericRepository<Projects> _projectsRepository;
    private readonly IUserAccessor _userAccessor;

    public ContractRepository(
        IUserAccessor userAccessor,
        IGenericRepository<Entities.Contracts> contractsRepository,
        IGenericRepository<FreelancerProfiles> freelancerProfilesRepository,
        IGenericRepository<ClientProfiles> clientProfilesRepository,
        IGenericRepository<Projects> projectsRepository)
    {
        ArgumentNullException.ThrowIfNull(userAccessor, nameof(userAccessor));
        ArgumentNullException.ThrowIfNull(contractsRepository, nameof(contractsRepository));
        ArgumentNullException.ThrowIfNull(freelancerProfilesRepository, nameof(freelancerProfilesRepository));
        ArgumentNullException.ThrowIfNull(clientProfilesRepository, nameof(clientProfilesRepository));
        ArgumentNullException.ThrowIfNull(projectsRepository, nameof(projectsRepository));
        _contractsRepository = contractsRepository;
        _freelancerProfilesRepository = freelancerProfilesRepository;
        _clientProfilesRepository = clientProfilesRepository;
        _projectsRepository = projectsRepository;
        _userAccessor = userAccessor;
    }

    public async Task CreateContractAsync(CreateContractCommand createContractCommand, CancellationToken cancellationToken)
    {
        var freelancer = await _freelancerProfilesRepository.Query()
            .Where(x => x.Users!.UserName == createContractCommand.CreateContractRequest.FreelancerName)
            .Include(x => x.Users)
            .FirstOrDefaultAsync(cancellationToken);

        if (freelancer is null)
            throw new NotFoundException(
                $"{nameof(FreelancerProfiles)} with {nameof(FreelancerProfiles.Users.UserName)}: {createContractCommand.CreateContractRequest.FreelancerName} doe not exist.");

        var client = await _clientProfilesRepository.Query()
            .Where(x => x.Users!.UserName == _userAccessor.GetUsername())
            .Include(x => x.Users)
            .FirstOrDefaultAsync(cancellationToken);
        if (client is null)
            throw new NotFoundException(
                $"{nameof(ClientProfiles)} with {nameof(ClientProfiles.Users.UserName)}: {_userAccessor.GetUsername()} doe not exist.");

        var project = await _projectsRepository.Query()
            .Where(x => x.Title == createContractCommand.CreateContractRequest.ProjectName)
            .FirstOrDefaultAsync(cancellationToken);
        if (project is null)
            throw new NotFoundException(
                $"{nameof(Projects)} with {nameof(Projects.Title)}: {createContractCommand.CreateContractRequest.ProjectName} doe not exist.");

        var contract = createContractCommand.CreateContractRequest.Adapt<Entities.Contracts>();
        contract.ProjectId = project.Id;
        contract.ClientId = client.Id;
        contract.FreelancerId = freelancer.Id;
        contract.Status = "Signed";
        await _contractsRepository.CreateAsync(contract, cancellationToken);
    }

    public async Task<ContractsDto> GetContractByIdAsync(GetContractByIdQuery query,
        CancellationToken cancellationToken)
    {
        var contract = await _contractsRepository.Query()
            .Where(x => x.Id == query.Id)
            .Include(x => x.Project)
            .Include(x => x.Client)
            .ThenInclude(c => c.Users)
            .Include(x => x.Freelancer)
            .ThenInclude(f => f.Users)
            .FirstOrDefaultAsync(cancellationToken);
        if (contract is null)
            throw new NotFoundException(
                $"{nameof(Entities.Contracts)} with {nameof(Entities.Contracts.Id)}: {query.Id} doe not exist.");

        return contract.Adapt<ContractsDto>();
    }

    public async Task<PaginatedList<ContractsDto>> GetContractsAsync(GetContractsQuery query,
        CancellationToken cancellationToken)
    {
        var contractsQuery = _contractsRepository.Query()
            .Include(x => x.Client)
            .ThenInclude(x => x.Users)
            .Include(x => x.Freelancer)
            .ThenInclude(f => f.Users)
            .Include(x => x.Project)
            .ProjectToType<ContractsDto>();
        var count = await contractsQuery.CountAsync(cancellationToken);
        var items = await contractsQuery
            .Skip((query.PaginationParams.PageNumber - 1) * query.PaginationParams.PageSize)
            .Take(query.PaginationParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<ContractsDto>(items, count, query.PaginationParams.PageNumber,
            query.PaginationParams.PageSize);
    }

    public async Task UpdateContractAsync(UpdateContractCommand updateContractCommand,
        CancellationToken cancellationToken)
    {
        var contract = await _contractsRepository.Query()
            .Where(x => x.Id == updateContractCommand.Id)
            .Include(x => x.Project)
            .Include(x => x.Client)
            .ThenInclude(x => x.Projects)
            .Include(x => x.Freelancer)
            .ThenInclude(x => x.Projects)
            .FirstOrDefaultAsync(cancellationToken);
        if (contract is null)
            throw new NotFoundException(
                $"nameof(Entities.Contracts) with {nameof(Entities.Contracts.Id)}: {updateContractCommand.Id} doe not exist.");

        updateContractCommand.UpdateContractRequest.Adapt(contract);

        if (updateContractCommand.UpdateContractRequest.Status == "Signed"
            && contract.Freelancer.Projects is not null
            && contract.Client.Projects is not null)
        {
            contract.Freelancer.Projects.Add(contract.Project);
            contract.Client.Projects.Add(contract.Project);
        }

        _contractsRepository.Update(contract);
        _clientProfilesRepository.Update(contract.Client);
        _freelancerProfilesRepository.Update(contract.Freelancer);
    }


    public async Task DeleteContractAsync(DeleteContractCommand deleteContractCommand,
        CancellationToken cancellationToken)
    {
        var contractToDelete = await _contractsRepository.Query()
            .Where(x => x.Id == deleteContractCommand.Id)
            .Include(x => x.Project)
            .FirstOrDefaultAsync(cancellationToken);

        if (contractToDelete is null)
            throw new NotFoundException(
                $"{nameof(Entities.Contracts)} with {nameof(Entities.Contracts.Id)}: {deleteContractCommand.Id} doe not exist.");

        _contractsRepository.Delete(contractToDelete);
    }
}