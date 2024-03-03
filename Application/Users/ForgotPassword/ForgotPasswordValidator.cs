using FluentValidation;

namespace Application.Users.ForgotPassword;

public class ForgotPasswordValidator: AbstractValidator<ForgotPasswordQuery>
{
    public ForgotPasswordValidator()
    {
        RuleFor(q => q.Email)
            .NotEmpty()
            .EmailAddress();
    }
}