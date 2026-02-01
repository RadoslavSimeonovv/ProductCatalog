using MediatR;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Application.Messaging;

public interface ICommand : IRequest<Result>, BaseCommand
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, BaseCommand
{
}