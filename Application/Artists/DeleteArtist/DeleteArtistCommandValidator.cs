using FluentValidation;

namespace Application.Artists.DeleteArtist;

public class DeleteArtistCommandValidator : AbstractValidator<DeleteArtistCommand>
{
    public DeleteArtistCommandValidator()
    {
        RuleFor(a=>a.Id)
            .NotEmpty();
    }
}