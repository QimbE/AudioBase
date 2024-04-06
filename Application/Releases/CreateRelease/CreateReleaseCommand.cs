using MediatR;

namespace Application.Releases.CreateRelease;

public record CreateReleaseCommand(string Name, string CoverLink, DateOnly ReleaseDate, Guid AuthorId, int TypeId): IRequest<Result<bool>>;