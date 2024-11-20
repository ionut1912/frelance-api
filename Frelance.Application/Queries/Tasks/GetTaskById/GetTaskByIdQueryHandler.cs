using Frelance.Contracts.Exceptions;
using Frelance.Contracts.Responses.Tasks;
using Frelance.Infrastructure.Context;
using Frelance.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Mapster;
namespace Frelance.Application.Queries.Tasks.GetTaskById;

public class GetTaskByIdQueryHandler:IRequestHandler<GetTaskByIdQuery,GetTaskByIdResponse>
{
    private readonly FrelanceDbContext _context;

    public GetTaskByIdQueryHandler(FrelanceDbContext context)
    {
        _context = context;
    }
    public async Task<GetTaskByIdResponse> Handle(GetTaskByIdQuery request, CancellationToken cancellationToken)
    {
        var task = await _context.Tasks.AsNoTracking().Include(x=>x.Project).Include(x=>x.TimeLogs).FirstOrDefaultAsync(x=>x.Id==request.Id, cancellationToken);
        if (task is null)
        {
            throw new NotFoundException($"{nameof(ProjectTask)} with {nameof(ProjectTask.Id)} : '{request.Id}' does not exist");
        }
        return task.Adapt<GetTaskByIdResponse>();
    }
}