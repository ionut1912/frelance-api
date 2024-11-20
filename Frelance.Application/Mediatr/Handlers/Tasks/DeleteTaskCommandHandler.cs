using Frelance.Application.Mediatr.Commands.Tasks;
using Frelance.Contracts.Exceptions;
using Frelance.Infrastructure.Context;
using Frelance.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Frelance.Application.Mediatr.Handlers.Tasks;

public class DeleteTaskCommandHandler:IRequestHandler<DeleteTaskCommand,Unit>
{
    private readonly FrelanceDbContext _context;

    public DeleteTaskCommandHandler(FrelanceDbContext context)
    {
        _context = context;
    }
    public async Task<Unit> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        var projectTaskToDelete = await _context.Tasks.FirstOrDefaultAsync(x=>x.Id==request.Id,cancellationToken);
        if (projectTaskToDelete is null)
        {
            throw new NotFoundException($"{nameof(ProjectTask)} with {nameof(ProjectTask.Id)} : '{request.Id}' does not exist");
        }
        _context.Tasks.Remove(projectTaskToDelete);
        await _context.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}