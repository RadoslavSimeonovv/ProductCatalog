using FluentValidation;

namespace ProductCatalog.Application.Payment.CreatePayment;

public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
{
    public CreatePaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
           .NotEmpty()
           .WithMessage("OrderId is required.");

        RuleFor(x => x.Provider)
            .NotEmpty()
            .WithMessage("Provider is required.")
            .MaximumLength(50)
            .WithMessage("Provider cannot exceed 50 characters.");

        RuleFor(x => x.ProviderReference)
            .MaximumLength(100)
            .WithMessage("ProviderReference cannot exceed 100 characters.")
            .When(x => x.ProviderReference != null);

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty()
            .WithMessage("IdempotencyKey is required.")
            .MaximumLength(255)
            .WithMessage("IdempotencyKey cannot exceed 255 characters.")
            .Must(k => !string.IsNullOrWhiteSpace(k))
            .WithMessage("IdempotencyKey cannot be whitespace.");
    }
}
