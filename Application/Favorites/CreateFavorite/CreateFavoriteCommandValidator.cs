using FluentValidation;

namespace Application.Favorites.CreateFavorite;

public class CreateFavoriteCommandValidator: AbstractValidator<CreateFavoriteCommand>
{
    public CreateFavoriteCommandValidator()
    {
        RuleFor(cfc => cfc.UserId)
            .NotNull();
        
        RuleFor(cfc => cfc.TrackId)
            .NotNull();
    }
}