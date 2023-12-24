using System.Text.Json.Serialization;
using Domain.Abstractions;
using Throw;

namespace Domain.Users;

/// <summary>
/// Application user
/// </summary>
public class User
    : Entity
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
        } }

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

    protected User(string name, string email, string password, int roleId)
        : base(Guid.NewGuid())
    {
        Name = name;
        Email = email;
        Password = password;
        RoleId = roleId;
    }

    public static User Create(string name, string email, string password, int roleId)
    {
        return new(name, email, password, roleId);
    }

    public void Update(string name, string email, string password, int roleId)
    {
        Name = name;
        Email = email;
        Password = password;
        RoleId = roleId;
    }

    public override int GetHashCode()
    {
        return this.Id.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        // If the same type and the same Id
        return obj is User usr &&
               usr.Id == this.Id;
    }

    public override string ToString()
    {
        return $"Entity of type {nameof(User)} with Id = {this.Id}";
    }
}