namespace Application.Authentication;
public record UserResponse(Guid UserId, string Username, int RoleId, string AccessToken, string RefreshToken);