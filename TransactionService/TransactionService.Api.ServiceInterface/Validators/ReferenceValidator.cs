using FluentValidation;

namespace TransactionService.Api.ServiceInterface;

public class ReferenceValidator : AbstractValidator<Ref>
{
    public ReferenceValidator()
    {
        RuleFor(c => c.Id).NotEmpty().Length(2, 255);
        RuleFor(c => c.Val).NotEmpty().Length(1, 255);
    }
}