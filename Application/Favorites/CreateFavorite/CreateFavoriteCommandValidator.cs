using FluentValidation;

namespace Application.Favorites.CreateFavorite;

public class CreateFavoriteCommandValidator: AbstractValidator<CreateFavoriteCommand>
{
    public CreateFavoriteCommandValidator()
    {
        RuleFor(cfc => cfc.UserToken)
            .NotEmpty();
        
        RuleFor(cfc => cfc.TrackId)
            .NotNull();
    }
}