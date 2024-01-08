using MediatR;

namespace Application.Authentication.Logout;

/// <summary>
/// Makes given refresh token expired.
/// </summary>
/// <param name="RefreshToken"></param>
public record LogoutCommand(string RefreshToken): IRequest<Result<bool>>;