using ServiceStack.FluentValidation;

namespace TransactionService.WebApi.ServiceInterface;

public class ReferenceValidator : AbstractValidator<Starnet.Common.Ref>
{
    public ReferenceValidator()
    {
        RuleFor(c => c.Id).NotEmpty().Length(2, 255);
        RuleFor(c => c.Val).NotEmpty().Length(1, 255);
    }
}