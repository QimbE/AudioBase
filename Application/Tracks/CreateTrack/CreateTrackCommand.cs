using MediatR;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace Application.Tracks.CreateTrack;

public record CreateTrackCommand(string Name, string AudioLink, string Duration, Guid ReleaseId, Guid GenreId): IRequest<Result<bool>>;