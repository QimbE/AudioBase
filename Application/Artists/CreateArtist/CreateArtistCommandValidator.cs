using FluentValidation;

namespace Application.Artists.CreateArtist;

public class CreateArtistCommandValidator : AbstractValidator<CreateArtistCommand>
{
    public CreateArtistCommandValidator()
    {
        RuleFor(a => a.Name)
            .NotEmpty()
            .MaximumLength(60);

        RuleFor(a => a.Description)
            .MaximumLength(2000);
        
        RuleFor(a => a.PhotoLink)
            .NotEmpty();
    }
}