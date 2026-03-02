using FluentValidation;

namespace ProductCatalog.Application.Order.SubmitOrderForPayment;

public class SubmitOrderForPaymentCommandValidator : AbstractValidator<SubmitOrderForPaymentCommand>
{
    public SubmitOrderForPaymentCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("OrderId is required.");

        RuleFor(x => x.Provider)
            .NotEmpty().WithMessage("Provider is required.")
            .Must(p => !string.IsNullOrWhiteSpace(p)).WithMessage("Provider cannot be whitespace.")
            .MaximumLength(50).WithMessage("Provider cannot exceed 50 characters.");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty().WithMessage("IdempotencyKey is required.")
            .Must(k => !string.IsNullOrWhiteSpace(k)).WithMessage("IdempotencyKey cannot be whitespace.")
            .MaximumLength(255).WithMessage("IdempotencyKey cannot exceed 255 characters.");
    }
}