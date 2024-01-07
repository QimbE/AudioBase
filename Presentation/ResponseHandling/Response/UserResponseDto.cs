using Application.Authentication.Register;

namespace Presentation.ResponseHandling.Response;

public record UserResponseDto(Guid UserId, string Username, string AccessToken)
{
    public UserResponseDto(UserResponse toMap)
        : this(toMap.UserId, toMap.Username, toMap.AccessToken)
    {
        
    }
};