using FluentValidation;
using M01.BaselineAPIProjectController.Requests;

namespace M01.BaselineAPIProjectController.Validations;

public class UpdateBudgetRequestValidator : AbstractValidator<UpdateBudgetRequest>
{
    public UpdateBudgetRequestValidator()
    {
        RuleFor(x => x.Budget).GreaterThanOrEqualTo(0);
    }
}

