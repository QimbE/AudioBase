using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Tracks.DeleteTrack;

public record DeleteTrackCommand(Guid Id): IRequest<Result<bool>>;