using System.Net;
using Application.Common.Models;
using FluentValidation.Results;

namespace Application.Common.Ultils;

public class ValidationHelper<T> where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationHelper(IValidator<T> validator)
    {
        _validator = validator;
    }

    public async Task<(bool IsValid, ResponseModel Response)> ValidateAsync(T model)
    {
        ValidationResult validationResult = await _validator.ValidateAsync(model);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
            var response = new ResponseModel(HttpStatusCode.BadRequest, "Validation Errors", errors);
            return (false, response);
        }

        return (true, null);
    }
}