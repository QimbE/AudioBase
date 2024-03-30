using MediatR;

namespace Application.Favorites.CreateFavorite;

public record CreateFavoriteCommand(Guid UserId, Guid TrackId): IRequest<Result<bool>>;