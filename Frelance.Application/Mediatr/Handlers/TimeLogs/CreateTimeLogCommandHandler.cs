using Frelance.Application.Mediatr.Commands.TimeLogs;
using Frelance.Application.Repositories;
using MediatR;

namespace Frelance.Application.Mediatr.Handlers.TimeLogs;

public class CreateTimeLogCommandHandler : IRequestHandler<CreateTimeLogCommand>
{
    private readonly ITimeLogRepository _timeLogRepository;
    private readonly IUnitOfWork _unitOfWork;


    public CreateTimeLogCommandHandler(ITimeLogRepository timeLogRepository, IUnitOfWork unitOfWork)
    {
        ArgumentNullException.ThrowIfNull(timeLogRepository, nameof(timeLogRepository));
        ArgumentNullException.ThrowIfNull(unitOfWork, nameof(unitOfWork));
        _timeLogRepository = timeLogRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CreateTimeLogCommand request, CancellationToken cancellationToken)
    {
        await _timeLogRepository.CreateTimeLogAsync(request, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}