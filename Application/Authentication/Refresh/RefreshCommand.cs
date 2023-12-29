using MediatR;

namespace Application.Authentication.Refresh;

public record RefreshCommand(string RefreshToken): IRequest<Result<TokenResponse>>;

public record TokenResponse(string AccessToken);