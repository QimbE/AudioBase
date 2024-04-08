using MediatR;

namespace Application.Tracks.UpdateTrack;

public record UpdateTrackCommand(Guid Id, string Name, string AudioLink, string Duration, Guid ReleaseId, Guid GenreId): IRequest<Result<bool>>;