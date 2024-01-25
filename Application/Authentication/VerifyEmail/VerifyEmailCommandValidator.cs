using FluentValidation;

namespace Application.Authentication.VerifyEmail;

public class VerifyEmailCommandValidator: AbstractValidator<VerifyEmailCommand>
{
    public VerifyEmailCommandValidator()
    {
        RuleFor(c => c.Token)
            .NotEmpty();
    }
}