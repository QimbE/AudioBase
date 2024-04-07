using FluentValidation;

namespace Application.Tracks.DeleteTrack;

public class DeleteTrackCommandValidator: AbstractValidator<DeleteTrackCommand>
{
    public DeleteTrackCommandValidator()
    {
        RuleFor(t => t.Id).NotEmpty();
    }
}