using FluentValidation;

namespace Application.Authentication.Refresh;

public class RefreshCommandValidator: AbstractValidator<RefreshCommand>
{
    public RefreshCommandValidator()
    {
        RuleFor(r => r.RefreshToken)
            .NotEmpty();
    }
}