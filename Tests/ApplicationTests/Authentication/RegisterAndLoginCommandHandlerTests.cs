using Application.Authentication;
using Application.Authentication.Register;
using Domain.Users;
using Domain.Users.Exceptions;
using FluentAssertions;
using Infrastructure.Authentication;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace ApplicationTests.Authentication;

public class RegisterAndLoginCommandHandlerTests
{
    private readonly TestDbContext _context;
    private readonly IJwtProvider _jwtProvider;
    private readonly IHashProvider _hashProvider;
    
    public RegisterAndLoginCommandHandlerTests()
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder
            .UseInMemoryDatabase("Test");

        _context = new TestDbContext(builder.Options);
        
        var sectionMock = Substitute.For<IConfigurationSection>();

        sectionMock["Key"].Returns("hehehehuhhehehehehehehehuhhwhwhwhhh");
        sectionMock["Issuer"].Returns("huh");
        sectionMock["Audience"].Returns("bimbimbambam");
        sectionMock["ExpiryTime"].Returns("00:30:00");
        
        var configMock = Substitute.For<IConfiguration>();

        configMock.GetSection("JwtSettings").Returns(sectionMock);

        _jwtProvider = new JwtProvider(configMock);
        _hashProvider = new HashProvider();
    }

    [Fact]
    public async Task Register_Should_ReturnException_OnDuplicateEmail()
    {
        // Arrange
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();

        var email = "blabla@mail.ru";
        _context.Users.Add(User.Create("123", email, "123", 1));
        await _context.SaveChangesAsync();
        
        var request = new RegisterCommand("Boban", email, "12345678");

        var handler = new RegisterCommandHandler(_context, _hashProvider, _jwtProvider);

        // Act
        var result = await handler.Handle(request, default);
        
        // Assert
        result.IsFaulted.Should().BeTrue();
        
        var exception = result.Match<Exception>(
            success => new Exception(),
            failure => failure
            );
        
        exception.Should().BeOfType<UserWithTheSameEmailException>();
    }

    ~RegisterAndLoginCommandHandlerTests()
    {
        _context.Dispose();
    }
}