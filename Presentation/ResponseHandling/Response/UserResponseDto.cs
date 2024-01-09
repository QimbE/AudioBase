using System.Text.Json.Serialization;
using Application.Authentication;
using Application.Authentication.Register;

namespace Presentation.ResponseHandling.Response;

public record UserResponseDto
{
    public Guid UserId { get; init; }
    public string Username { get; init; }
    public string AccessToken { get; init; }
    
    public UserResponseDto(UserResponse toMap)
        : this(toMap.UserId, toMap.Username, toMap.AccessToken)
    {
        
    }

    [JsonConstructor]
    public UserResponseDto(Guid UserId, string Username, string AccessToken)
    {
        this.UserId = UserId;
        this.Username = Username;
        this.AccessToken = AccessToken;
    }
};