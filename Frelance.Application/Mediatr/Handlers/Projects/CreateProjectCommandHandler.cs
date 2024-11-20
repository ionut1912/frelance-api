using Frelance.Application.Mediatr.Commands.Projects;
using Frelance.Infrastructure.Context;
using Frelance.Infrastructure.Entities;
using MediatR;

namespace Frelance.Application.Mediatr.Handlers.Projects;

public class CreateProjectCommandHandler:IRequestHandler<CreateProjectCommand,int>
{
    public readonly FrelanceDbContext _frelanceDbContext;

    public CreateProjectCommandHandler(FrelanceDbContext frelanceDbContext)
    {
        _frelanceDbContext = frelanceDbContext;
    }

    public async Task<int> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = new Project
        {
            CreatedAt = DateTime.Now.ToUniversalTime(),
            Title = request.Title,
            Description = request.Description,
            Deadline = request.Deadline,
            Technologies = request.Technologies,
            Budget = request.Budget
        };
        await _frelanceDbContext.Projects.AddAsync(project,cancellationToken);
        await _frelanceDbContext.SaveChangesAsync(cancellationToken);
        return project.Id;
    }
    
}