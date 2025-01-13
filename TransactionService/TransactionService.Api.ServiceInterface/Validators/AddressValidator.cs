using FluentValidation;

namespace TransactionService.Api.ServiceInterface;

public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(c => c.Street).NotEmpty().NotEmpty().Length(2, 150);
        RuleFor(c => c.City).NotEmpty().Length(2, 150);
        RuleFor(c => c.State).NotEmpty().Length(2, 150);
        RuleFor(c => c.PostalCode).NotEmpty().Length(2, 150);
        RuleFor(c => c.Country).NotEmpty().SetValidator(new ReferenceValidator());
    }
}