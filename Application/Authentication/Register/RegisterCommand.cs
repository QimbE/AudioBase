using MediatR;

namespace Application.Authentication.Register;

public record RegisterCommand(string Name, string Email, string Password): IRequest<Result<UserResponse>>;

public record UserResponse(Guid UserId, string Username, string AccessToken, string RefreshToken);