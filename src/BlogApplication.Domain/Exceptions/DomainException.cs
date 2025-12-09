namespace BlogApplication.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }
}

public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}

public class ValidationException : DomainException
{
    public ValidationException(string message) : base(message)
    {
    }
}
