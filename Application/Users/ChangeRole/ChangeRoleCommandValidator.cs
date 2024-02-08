using Domain.Users;
using FluentValidation;

namespace Application.Users.ChangeRole;

public class ChangeRoleCommandValidator: AbstractValidator<ChangeRoleCommand>
{
    public ChangeRoleCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();

        RuleFor(c => c.RoleName)
            // Must be convertable to actual role
            .Must(x => Role.TryFromName(x, true, out var _))
            .WithMessage(x => $"There is no role with name {x.RoleName}");
    }
}