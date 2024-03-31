using FluentValidation;

namespace Application.Favorites.DeleteFavorite;

public class DeleteFavoriteCommandValidator: AbstractValidator<DeleteFavoriteCommand>
{
    public DeleteFavoriteCommandValidator()
    {
        RuleFor(dfc => dfc.UserToken)
            .NotEmpty();
        
        RuleFor(dfc => dfc.TrackId)
            .NotNull();
    }
}