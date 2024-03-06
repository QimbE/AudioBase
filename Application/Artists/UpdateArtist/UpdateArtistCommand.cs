using MediatR;

namespace Application.Artists.UpdateArtist;

public record UpdateArtistCommand (Guid Id, string Name, string Description, string PhotoLink): IRequest<Result<bool>>;