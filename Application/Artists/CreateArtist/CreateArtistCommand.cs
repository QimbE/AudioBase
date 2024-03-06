using MediatR;

namespace Application.Artists.CreateArtist;

public record CreateArtistCommand(string Name, string? Description, string PhotoLink): IRequest<Result<bool>>;