using FluentValidation;
using TransactionService.Api.ServiceModel.Commands;

namespace TransactionService.Api.ServiceInterface;

public class RegisterOrganizationValidator : AbstractValidator<RegisterOrganization>
{
    public RegisterOrganizationValidator()
    {
        RuleFor(c => c.Id).NotEmpty().Matches("Organizations-\\w");
        RuleFor(c => c.Name).NotEmpty().Length(2, 150);
        RuleFor(c => c.Address).NotEmpty().SetValidator(new AddressValidator());
    }
}