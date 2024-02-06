using MediatR;

namespace Application.Authentication.RequestVerification;

public record RequestVerificationQuery(Guid UserId): IRequest<Result<bool>>;