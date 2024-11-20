using Frelance.Application.Mediatr.Commands.Projects;
using Frelance.Application.Mediatr.Queries.Projects;
using Frelance.Contracts.Dtos;
using Frelance.Contracts.Responses.Common;
using Frelance.Contracts.Responses.Projects;

namespace Frelance.Application.Repositories;

public interface IProjectRepository
{
    Task AddProjectAsync(CreateProjectCommand createProjectCommand,CancellationToken cancellationToken);
    Task UpdateProjectAsync(UpdateProjectCommand updateProjectCommand,CancellationToken cancellationToken);
    Task DeleteProjectAsync(DeleteProjectCommand deleteProjectCommand,CancellationToken cancellationToken);
    Task<GetProjectByIdResponse> FindProjectByIdAsync(GetProjectByIdQuery getProjectByIdQuery,CancellationToken cancellationToken);
    Task<PaginatedList<ProjectDto>> FindProjectsAsync(GetProjectsQuery getProjectsQuery,CancellationToken cancellationToken);
    
}