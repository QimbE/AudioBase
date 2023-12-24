using Domain.Users;
using FluentAssertions;

namespace DomainTests;

public class UserTests
{
    [Fact]
    public void Create_ShouldThrowException_OnNullOrWhiteSpaceName()
    {
        // Arrange
        var action = () => User.Create("  ", "123", "blablabla", 1);
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_OnNullOrWhiteSpaceEmail()
    {
        // Arrange
        var action = () => User.Create("123", "  ", "blablabla", 1);
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_OnNullOrWhiteSpacePassword()
    {
        // Arrange
        var action = () => User.Create("123", "blablabla", "  ", 1);
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Create_ShouldThrowException_OnNonexistentRoleId()
    {
        // Arrange
        var action = () => User.Create("123", "blablabla", "12345", -123341);
        
        // Assert
        action.Should().Throw<ArgumentException>();
    }
    
    [Fact]
    public void Update_ShouldNotThrowException_OnValidInput()
    {
        // Arrange
        var user = User.Create("123", "blablabla", "12345", Role.List.First());
        var expectedUser = User.Create("test", "test", "test", Role.List.First());
        
        var action = () => user.Update("test", "test", "test", Role.List.First());
        
        // Assert
        action.Should().NotThrow<ArgumentException>();

        user.Should().Match<User>(u =>
            u.Name == expectedUser.Name &&
            u.Email == expectedUser.Email && 
            u.Password == expectedUser.Password &&
            u.RoleId == expectedUser.RoleId
            );
    }
}