using FluentValidation;

namespace Application.Favorites.CreateFavorite;

public class AddFavoriteCommandValidator: AbstractValidator<AddFavoriteCommand>
{
    public AddFavoriteCommandValidator()
    {
        RuleFor(cfc => cfc.UserId)
            .NotEmpty();
        
        RuleFor(cfc => cfc.TrackId)
            .NotEmpty();
    }
}