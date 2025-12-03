namespace BankApi.Domain.Exceptions;

/// <summary>
/// Base exception for domain-specific errors
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exception thrown when insufficient balance is available
/// </summary>
public class InsufficientBalanceException : DomainException
{
    public InsufficientBalanceException(decimal available, decimal required)
        : base($"Insufficient balance. Available: {available}, Required: {required}")
    {
    }
}

/// <summary>
/// Exception thrown when an invalid operation is attempted
/// </summary>
public class InvalidOperationDomainException : DomainException
{
    public InvalidOperationDomainException(string message) : base(message)
    {
    }
}

/// <summary>
/// Exception thrown when validation fails
/// </summary>
public class ValidationException : DomainException
{
    public ValidationException(string message) : base(message)
    {
    }
}
