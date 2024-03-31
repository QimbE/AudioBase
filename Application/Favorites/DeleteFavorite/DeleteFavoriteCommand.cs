using MediatR;

namespace Application.Favorites.DeleteFavorite;

public record DeleteFavoriteCommand(string UserToken, Guid TrackId): IRequest<Result<bool>>;