using FluentValidation;

namespace Application.Labels.CreateLabel;

public class CreateLabelCommandValidator: AbstractValidator<CreateLabelCommand>
{
    public CreateLabelCommandValidator()
    {
        RuleFor(l => l.Name)
            .NotEmpty()
            .MaximumLength(60);

        RuleFor(l => l.Description)
            .MaximumLength(2000);
        
        RuleFor(l => l.PhotoLink)
            .NotEmpty();
    }
}