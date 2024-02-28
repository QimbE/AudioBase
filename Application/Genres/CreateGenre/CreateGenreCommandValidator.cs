using FluentValidation;

namespace Application.Genres.CreateGenre;

public class CreateGenreCommandValidator: AbstractValidator<CreateGenreCommand>
{
    public CreateGenreCommandValidator()
    {
        RuleFor(g => g.Name).NotEmpty();
    }
}