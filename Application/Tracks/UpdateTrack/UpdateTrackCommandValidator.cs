using FluentValidation;

namespace Application.Tracks.UpdateTrack;

public class UpdateTrackCommandValidator: AbstractValidator<UpdateTrackCommand>
{
    public UpdateTrackCommandValidator()
    {
        RuleFor(t => t.Id)
            .NotEmpty();
        
        RuleFor(t => t.Name)
            .NotEmpty()
            .MaximumLength(60);

        RuleFor(t => t.AudioLink)
            .NotEmpty();

        RuleFor(t => t.Duration)
            .Matches(@"^(\d{2}:\d{2}:\d{2})$");

        RuleFor(t => t.ReleaseId)
            .NotEmpty();

        RuleFor(t => t.GenreId)
            .NotEmpty();
    }
}
