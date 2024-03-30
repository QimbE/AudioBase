using FluentValidation;

namespace Application.Labels.UpdateLabel;

public class UpdateLabelCommandValidator : AbstractValidator<UpdateLabelCommand>
{
    public UpdateLabelCommandValidator()
    {
        RuleFor(a => a.Id)
            .NotEmpty();

        RuleFor(a => a.Name)
            .NotEmpty()
            .MaximumLength(60);

        RuleFor(a => a.Description)
            .MaximumLength(2000);

        RuleFor(a => a.PhotoLink)
            .NotEmpty();
    }
}