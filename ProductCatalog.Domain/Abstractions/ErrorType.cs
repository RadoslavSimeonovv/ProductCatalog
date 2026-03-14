namespace ProductCatalog.Domain.Abstractions;

public enum ErrorType
{
    NotFound,
    Validation,
    Failure,
    Conflict,
    Unauthorized
}