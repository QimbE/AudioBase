using MediatR;

namespace Application.Artists.DeleteArtist;

public record DeleteArtistCommand (Guid Id): IRequest<Result<bool>>;