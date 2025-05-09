namespace Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException() : base() { }
    public ForbiddenAccessException(string message) : base(message)
    {
        // Add any type-specific logic.
    }

    public ForbiddenAccessException(string message, Exception innerException) :
       base(message, innerException)
    {
        // Add any type-specific logic for inner exceptions.
    }
}
