using Frelance.Application.Mediatr.Queries.Projects;
using Frelance.Contracts.Exceptions;
using Frelance.Contracts.Responses.Projects;
using Frelance.Infrastructure.Context;
using Frelance.Infrastructure.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Frelance.Application.Mediatr.Handlers.Projects;

public class GetProjectByIdQueryHandler:IRequestHandler<GetProjectByIdQuery,GetProjectByIdResponse>
{
    private readonly FrelanceDbContext _context;

    public GetProjectByIdQueryHandler(FrelanceDbContext context)
    {
        _context = context;
    }
    public async Task<GetProjectByIdResponse> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project=await _context.Projects.AsNoTracking().Include(x=>x.Tasks).FirstOrDefaultAsync(x=>x.Id==request.Id, cancellationToken);
        if (project is null)
        {
            throw new NotFoundException($"{nameof(Project)} with {nameof(Project.Id)} : '{request.Id}' does not exist");
        }

        return project.Adapt<GetProjectByIdResponse>();
    }
}