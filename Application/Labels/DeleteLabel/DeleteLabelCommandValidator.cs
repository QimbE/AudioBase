using FluentValidation;

namespace Application.Labels.DeleteLabel;

public class DeleteLabelCommandValidator : AbstractValidator<DeleteLabelCommand>
{
    public DeleteLabelCommandValidator()
    {
        RuleFor(l=>l.Id)
            .NotEmpty();
    }
}