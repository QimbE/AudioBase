using FluentValidation;

namespace Application.Releases.UpdateRelease;

public class UpdateReleaseCommandValidator: AbstractValidator<UpdateReleaseCommand>
{
    public UpdateReleaseCommandValidator()
    {
        RuleFor(r => r.Id)
            .NotEmpty();
        
        RuleFor(r => r.Name)
            .NotEmpty()
            .MaximumLength(60);

        RuleFor(r => r.CoverLink)
            .NotEmpty();

        RuleFor(r => r.ReleaseDate)
            .Matches(@"\d{4}-\d{2}-\d{2}");

        RuleFor(r => r.AuthorId)
            .NotEmpty();
        
        RuleFor(r => r.TypeId)
            .NotEmpty();
    }
}