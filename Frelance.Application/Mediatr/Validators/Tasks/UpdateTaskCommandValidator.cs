using FluentValidation;
using Frelance.Application.Mediatr.Commands.Tasks;
using JetBrains.Annotations;

namespace Frelance.Application.Mediatr.Validators.Tasks;

[UsedImplicitly]
public class UpdateTaskCommandValidator : AbstractValidator<UpdateTaskCommand>
{
    public UpdateTaskCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.UpdateProjectTaskRequest.Title).MaximumLength(50);
        RuleFor(x => x.UpdateProjectTaskRequest.Description).MaximumLength(500);
    }
}