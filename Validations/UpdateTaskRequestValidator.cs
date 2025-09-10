using FluentValidation;
using M01.BaselineAPIProjectController.Requests;

namespace M01.BaselineAPIProjectController.Validations;

public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskRequest>
{
    public UpdateTaskRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Status).IsInEnum();
    }
}

