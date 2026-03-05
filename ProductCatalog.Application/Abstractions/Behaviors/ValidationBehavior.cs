using FluentValidation;
using MediatR;
using ProductCatalog.Application.Abstractions.Messaging;
using ProductCatalog.Application.Exceptions;
using ValidationException = ProductCatalog.Application.Exceptions.ValidationException;

namespace ProductCatalog.Application.Abstractions.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var results = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var validationErrors = results
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .Select(f => new ValidationError(f.PropertyName, f.ErrorMessage))
            .ToList();

        if (validationErrors.Any())
        {
            throw new ValidationException(validationErrors);

        }

        return await next();
    }
}
