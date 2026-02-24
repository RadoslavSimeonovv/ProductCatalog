using MediatR;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand
{
}

public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{
}