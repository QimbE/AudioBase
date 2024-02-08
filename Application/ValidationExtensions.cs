using FluentValidation;

namespace Application;

/// <summary>
/// Common validation extension methods
/// </summary>
public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, string> IsPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty()
            .MinimumLength(8)
            .MaximumLength(30);
    }
}