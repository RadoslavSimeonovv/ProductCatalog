namespace ProductCatalog.Domain.Abstractions;

public record Error(string Code, string Message, ErrorType Type = ErrorType.Failure)
{
    public static Error None => new Error(string.Empty, "No Error");

    public static Error NullValue => new Error("Error.NullValue", "Null Value was provided", ErrorType.Failure);
}