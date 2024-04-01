using FluentValidation;

namespace Application.Favorites.DeleteFavorite;

public class DeleteFavoriteCommandValidator: AbstractValidator<DeleteFavoriteCommand>
{
    public DeleteFavoriteCommandValidator()
    {
        RuleFor(dfc => dfc.UserId)
            .NotNull();
        
        RuleFor(dfc => dfc.TrackId)
            .NotNull();
    }
}