namespace Application.Authentication;
public record UserResponse(Guid UserId, string Username, string AccessToken, string RefreshToken);