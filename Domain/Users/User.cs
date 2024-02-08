using System.Text.Json.Serialization;
using Domain.Abstractions;
using Domain.Users.Events;
using Throw;

namespace Domain.Users;

/// <summary>
/// Application user
/// </summary>
public class User
    : Entity<User>
{
    private string _name;
    public string Name
    {
        get => _name;
        protected set
        {
            _name = value.Throw().IfNullOrWhiteSpace(x => x);
        }
    }

    private string _email;
    public string Email
    {
        get => _email;
        protected set
        {
            _email = value.Throw().IfNullOrWhiteSpace(x => x);
        }
    }
    
    /// <summary>
    /// Have user verified his/her email?
    /// </summary>
    public bool IsVerified { get; protected set; }

    private string _password;
    /// <summary>
    /// Password hash
    /// </summary>
    [JsonIgnore]
    public string Password
    {
        get => _password;
        protected set
        {
            _password = value.Throw().IfNullOrWhiteSpace(x => x);
        }
    }

    private int _roleId;
    public int RoleId
    {
        get => _roleId;
        protected set
        {
            // if there is no such a role
            _roleId = value.Throw().IfFalse(x => Role.TryFromValue(value, out var _));
        }
    }

    public Role Role { get; protected set; }

    public RefreshToken RefreshToken { get; protected set; }

    protected User()
    {
        
    }

    protected User(string name, string email, string password, int roleId, bool isVerified)
        : base(Guid.NewGuid())
    {
        Name = name;
        Email = email;
        Password = password;
        RoleId = roleId;
        IsVerified = isVerified;
    }

    public static User Create(
        string name,
        string email,
        string password,
        int roleId
        )
    {
        User result = new(name, email, password, roleId, false);

        result.RefreshToken = RefreshToken.Create("sample", result.Id);
        
        result.RaiseEvent(new UserCreatedDomainEvent(result.Id));
        
        return result;
    }

    public void Update(string name, string email, string password, int roleId)
    {
        Name = name;
        Email = email;
        Password = password;
        RoleId = roleId;
    }

    /// <summary>
    /// Sets IsVerified to true
    /// </summary>
    public void VerifyEmail()
    {
        IsVerified = true;
    }
    
    public void RequestVerification()
    {
        RaiseEvent(new UserCreatedDomainEvent(this.Id));
    }
}