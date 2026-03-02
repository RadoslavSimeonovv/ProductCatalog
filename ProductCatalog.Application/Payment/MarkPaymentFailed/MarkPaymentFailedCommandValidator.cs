using FluentValidation;

namespace ProductCatalog.Application.Payment.MarkPaymentFailed;

public class MarkPaymentFailedCommandValidator : AbstractValidator<MarkPaymentFailedCommand>
{
    public MarkPaymentFailedCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .WithMessage("PaymentId is required.");

        RuleFor(x => x.Reason)
           .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
    }
}