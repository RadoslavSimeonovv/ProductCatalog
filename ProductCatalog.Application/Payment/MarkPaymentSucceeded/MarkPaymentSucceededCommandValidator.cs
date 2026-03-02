using FluentValidation;

namespace ProductCatalog.Application.Payment.MarkPaymentSucceeded;

public class MarkPaymentSucceededCommandValidator : AbstractValidator<MarkPaymentSucceededCommand>
{
    public MarkPaymentSucceededCommandValidator()
    {
        RuleFor(x => x.PaymentId)
            .NotEmpty()
            .WithMessage("PaymentId is required.");
    }
}
