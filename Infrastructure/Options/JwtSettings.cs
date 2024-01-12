using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options;

public class JwtSettings
{
    [Required]
    public string Issuer { get; init; }
    
    [Required]
    public string Audience { get; init; }
    
    [Required]
    [MinLength(32)]
    public string Key { get; init; }
    
    [Required]
    public TimeSpan ExpiryTime { get; init; }
}