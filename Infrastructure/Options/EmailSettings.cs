using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Options;

public class EmailSettings
{
    [Required]
    public string DisplayName { get; init; }
    
    /// <summary>
    /// Actual email address of sender
    /// </summary>
    [Required]
    [EmailAddress]
    public string From { get; init; }
    
    /// <summary>
    /// Smtp client login
    /// </summary>
    [Required]
    public string UserName { get; init; }
    
    /// <summary>
    /// Smtp client password
    /// </summary>
    [Required]
    public string Password { get; init; }
    
    [Required]
    public string Host { get; init; }
    
    [Required]
    public int Port { get; init; }
    
    /// <summary>
    /// Should we send emails for real or not
    /// </summary>
    [Required]
    public bool IsProduction { get; init; }
    
    /// <summary>
    /// Full path to verification page
    /// </summary>
    /// <example>
    /// "https://audio-base.ru/user/verifyemail"
    /// </example>
    [Required]
    public string VerificationPageUrl { get; init; }
}