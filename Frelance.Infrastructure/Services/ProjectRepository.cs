using Frelance.Application.Mediatr.Commands.Projects;
using Frelance.Application.Mediatr.Queries.Projects;
using Frelance.Application.Repositories;
using Frelance.Contracts.Dtos;
using Frelance.Contracts.Exceptions;
using Frelance.Contracts.Responses.Common;
using Frelance.Infrastructure.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Frelance.Infrastructure.Services;

public class ProjectRepository : IProjectRepository
{
    private readonly IGenericRepository<Projects> _projectRepository;
    private readonly IGenericRepository<ProjectTechnologies> _projectTechnologyRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProjectRepository(IGenericRepository<Projects> projectRepository,
        IGenericRepository<ProjectTechnologies> projectTechnologyRepository,
        IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(projectRepository, nameof(projectRepository));
        ArgumentNullException.ThrowIfNull(projectTechnologyRepository, nameof(projectTechnologyRepository));
        ArgumentNullException.ThrowIfNull(unitOfWork, nameof(unitOfWork));
        _projectRepository = projectRepository;
        _projectTechnologyRepository = projectTechnologyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task CreateProjectAsync(CreateProjectCommand createProjectCommand, CancellationToken cancellationToken)
    {
        var project = createProjectCommand.CreateProjectRequest.Adapt<Projects>();
        await _projectRepository.CreateAsync(project, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (project.Technologies.Count != 0)
        {
            project.Technologies.ForEach(tech =>
            {
                tech.ProjectId = project.Id;
                tech.Id = 0;
            });
            await _projectTechnologyRepository.CreateRangeAsync(project.Technologies, cancellationToken);
        }
    }


    public async Task UpdateProjectAsync(UpdateProjectCommand updateProjectCommand, CancellationToken cancellationToken)
    {
        var projectToUpdate = await _projectRepository.Query()
            .Where(x => x.Id == updateProjectCommand.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (projectToUpdate is null)
            throw new NotFoundException(
                $"{nameof(Projects)} with {nameof(Projects.Id)} : '{updateProjectCommand.Id}' does not exist");

        updateProjectCommand.UpdateProjectRequest.Adapt(projectToUpdate);
        _projectRepository.Update(projectToUpdate);
    }

    public async Task DeleteProjectAsync(DeleteProjectCommand deleteProjectCommand, CancellationToken cancellationToken)
    {
        var projectToDelete = await _projectRepository.Query()
            .Where(x => x.Id == deleteProjectCommand.Id)
            .FirstOrDefaultAsync(cancellationToken);
        if (projectToDelete is null)
            throw new NotFoundException(
                $"{nameof(Projects)} with {nameof(Projects.Id)} : '{deleteProjectCommand.Id}' does not exist");
        _projectRepository.Delete(projectToDelete);
    }

    public async Task<ProjectDto> FindProjectByIdAsync(GetProjectByIdQuery getProjectByIdQuery,
        CancellationToken cancellationToken)
    {
        var project = await _projectRepository.Query()
            .Where(x => x.Id == getProjectByIdQuery.Id)
            .Include(x => x.Tasks)
            .Include(x => x.Proposals)
            .Include(x => x.Contracts)
            .Include(x => x.Invoices)
            .FirstOrDefaultAsync(cancellationToken);
        if (project is null)
            throw new NotFoundException(
                $"{nameof(Projects)} with {nameof(Projects.Id)} : '{getProjectByIdQuery.Id}' does not exist");

        return project.Adapt<ProjectDto>();
    }

    public async Task<PaginatedList<ProjectDto>> FindProjectsAsync(GetProjectsQuery getProjectsQuery,
        CancellationToken cancellationToken)
    {
        var projectQuery = _projectRepository.Query()
            .Include(x => x.Tasks)
            .Include(x => x.Invoices)
            .Include(x => x.Contracts)
            .Include(x => x.Proposals)
            .ProjectToType<ProjectDto>();
        var count = await projectQuery.CountAsync(cancellationToken);
        var items = await projectQuery
            .Skip((getProjectsQuery.PaginationParams.PageNumber - 1) * getProjectsQuery.PaginationParams.PageSize)
            .Take(getProjectsQuery.PaginationParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<ProjectDto>(items, count, getProjectsQuery.PaginationParams.PageNumber,
            getProjectsQuery.PaginationParams.PageSize);
    }
}