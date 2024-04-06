using FluentValidation;

namespace Application.Releases.CreateRelease;

public class CreateReleaseCommandValidatior: AbstractValidator<CreateReleaseCommand>
{
    public CreateReleaseCommandValidatior()
    {
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