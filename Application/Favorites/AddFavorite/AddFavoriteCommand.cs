using MediatR;

namespace Application.Favorites.CreateFavorite;

public record AddFavoriteCommand(Guid UserId, Guid TrackId): IRequest<Result<bool>>;