using FluentValidation;

namespace Application.Users.ChangePassword;

public class ChangePasswordCommandValidator: AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(c => c.UserId)
            .NotEmpty();

        RuleFor(c => c.OldPassword)
            .IsPassword();

        RuleFor(c => c.NewPassword)
            .IsPassword();
    }
}