using FluentValidation;

namespace ProductCatalog.Application.Order.GetOrderById;

public class GetOrderByIdQueryValidator : AbstractValidator<GetOrderByIdQuery>
{
    public GetOrderByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Order ID must not be empty.");
    }
}