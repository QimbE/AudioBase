﻿using FluentValidation;

namespace Application.Authentication.Register;

public class RegisterCommandValidator: AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(r => r.Name)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(30);

        RuleFor(r => r.Password)
            .IsPassword();

        RuleFor(r => r.Email)
            .NotEmpty()
            .EmailAddress();
    }
}