using Application.Artists.DeleteArtist;
using FluentValidation;

namespace Application.Releases.DeleteRelease;

public class DeleteReleaseCommandValidator: AbstractValidator<DeleteReleaseCommand>
{
    public DeleteReleaseCommandValidator()
    {
        RuleFor(r => r.Id).NotEmpty();
    }
}