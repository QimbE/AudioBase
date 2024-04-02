using FluentValidation;

namespace Application.Favorites.DeleteFavorite;

public class DeleteFavoriteCommandValidator: AbstractValidator<DeleteFavoriteCommand>
{
    public DeleteFavoriteCommandValidator()
    {
        RuleFor(dfc => dfc.UserId)
            .NotEmpty();
        
        RuleFor(dfc => dfc.TrackId)
            .NotEmpty();
    }
}