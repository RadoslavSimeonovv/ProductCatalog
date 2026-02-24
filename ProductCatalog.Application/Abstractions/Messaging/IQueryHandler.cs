using MediatR;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}
