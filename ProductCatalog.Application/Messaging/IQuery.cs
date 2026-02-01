using MediatR;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Application.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
