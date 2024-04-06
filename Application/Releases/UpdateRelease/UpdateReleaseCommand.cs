using MediatR;

namespace Application.Releases.UpdateRelease;

public record UpdateReleaseCommand (Guid Id, string Name, string CoverLink, DateOnly ReleaseDate, Guid AuthorId, int TypeId): IRequest<Result<bool>>;