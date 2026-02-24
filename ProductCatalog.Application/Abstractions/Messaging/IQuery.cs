using MediatR;
using ProductCatalog.Domain.Abstractions;

namespace ProductCatalog.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}
