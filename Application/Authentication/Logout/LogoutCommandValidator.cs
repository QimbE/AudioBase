using FluentValidation;

namespace Application.Authentication.Logout;

public class LogoutCommandValidator: AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotEmpty();
    }
}