using FluentValidation;

namespace Application.Favorites.CreateFavorite;

public class CreateFavoriteCommandValidator: AbstractValidator<CreateFavoriteCommand>
{
    public CreateFavoriteCommandValidator()
    {
        RuleFor(cfc => cfc.UserId)
            .NotEmpty();
        
        RuleFor(cfc => cfc.TrackId)
            .NotEmpty();
    }
}