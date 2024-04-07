using MediatR;

namespace Application.Releases.DeleteRelease;

public record DeleteReleaseCommand(Guid Id): IRequest<Result<bool>>;