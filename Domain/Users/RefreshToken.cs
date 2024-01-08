using Domain.Abstractions;
using Throw;

namespace Domain.Users;

/// <summary>
/// Access refresh token for users
/// </summary>
public class RefreshToken
    : Entity<RefreshToken>
{
    private string _value;
    
    /// <summary>
    /// Actual token value
    /// </summary>
    public string Value
    {
        get => _value;
        protected set
        {
            _value = value.Throw().IfNullOrWhiteSpace(x => x);
        }
    }
    
    /// <summary>
    /// Expiry date
    /// </summary>
    public DateTime ExpirationDate { get; protected set; }
    
    public User Owner { get; protected set; }
    
    /// <summary>
    /// Token life time
    /// </summary>
    public const int LifeTime = 30;

    protected RefreshToken(string value, DateTime expirationDate, Guid userId)
        : base(userId)
    {
        Value = value;
        ExpirationDate = expirationDate;
    }
    
    protected RefreshToken()
    {
        
    }

    /// <summary>
    /// Create an instance of RefreshToken
    /// </summary>
    /// <param name="value">token value</param>
    /// <param name="userId">token's owner id</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">on attempt to create token consists of null or blank string</exception>
    public static RefreshToken Create(string value, Guid userId)
    {
        return new(
            value,
            DateTime.UtcNow.AddDays(LifeTime),
            userId
            );
    }

    public void Update(string value)
    {
        Value = value;
        ExpirationDate = DateTime.UtcNow.AddDays(LifeTime);
    }

    public void MakeExpire()
    {
        ExpirationDate = DateTime.UtcNow;
    }

    public override string ToString()
    {
        return base.ToString() + $" Value: {this.Value}";
    }
}