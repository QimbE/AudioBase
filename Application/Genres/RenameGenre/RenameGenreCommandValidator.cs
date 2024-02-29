using FluentValidation;

namespace Application.Genres.RenameGenre;

public class RenameGenreCommandValidator : AbstractValidator<RenameGenreCommand>
{
    public RenameGenreCommandValidator()
    {
        RuleFor(g => g.Id).NotEmpty();
        
        RuleFor(g => g.NewName).NotEmpty();
    }
}