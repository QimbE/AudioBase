﻿using FluentValidation;

namespace Application.Authentication.Login;

public class LoginCommandValidator: AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(l => l.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(l => l.Password).IsPassword();
    }
}