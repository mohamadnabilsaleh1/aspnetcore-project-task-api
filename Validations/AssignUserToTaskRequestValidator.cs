using FluentValidation;
using M01.BaselineAPIProjectController.Requests;

namespace M01.BaselineAPIProjectController.Validations;

public class AssignUserToTaskRequestValidator : AbstractValidator<AssignUserToTaskRequest>
{
    public AssignUserToTaskRequestValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

