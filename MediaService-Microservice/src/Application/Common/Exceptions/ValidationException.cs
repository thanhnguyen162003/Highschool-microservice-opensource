using FluentValidation.Results;

namespace Application.Common.Exceptions;

#pragma warning disable RCS1194
public class ValidationException() : Exception("One or more validation failures have occurred.")
{

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();
}
