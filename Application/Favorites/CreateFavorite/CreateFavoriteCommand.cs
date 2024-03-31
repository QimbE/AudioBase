using MediatR;

namespace Application.Favorites.CreateFavorite;

public record CreateFavoriteCommand(string UserToken, Guid TrackId): IRequest<Result<bool>>;