using FluentValidation;

namespace Application.Artists.UpdateArtist;

public class UpdateArtistCommandValidator: AbstractValidator<UpdateArtistCommand>
{
    public UpdateArtistCommandValidator()
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