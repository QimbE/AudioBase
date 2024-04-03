using MediatR;

namespace Application.Favorites.DeleteFavorite;

public record DeleteFavoriteCommand(Guid UserId, Guid TrackId): IRequest<Result<bool>>;