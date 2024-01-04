using Application.Authentication;
using Infrastructure.Authentication;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace ApplicationTests.Authentication;

public abstract class AuthTestingBase
{
    protected readonly TestDbContext Context;
    protected readonly IJwtProvider JwtProvider;
    protected readonly IHashProvider HashProvider;
    
    public AuthTestingBase()
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder
            .UseInMemoryDatabase("Test");

        Context = new TestDbContext(builder.Options);
        
        var sectionMock = Substitute.For<IConfigurationSection>();

        sectionMock["Key"].Returns("hehehehuhhehehehehehehehuhhwhwhwhhh");
        sectionMock["Issuer"].Returns("huh");
        sectionMock["Audience"].Returns("bimbimbambam");
        sectionMock["ExpiryTime"].Returns("00:30:00");
        
        var configMock = Substitute.For<IConfiguration>();

        configMock.GetSection("JwtSettings").Returns(sectionMock);

        JwtProvider = new JwtProvider(configMock);
        HashProvider = new HashProvider();
    }

    public Task RecreateDbContextAsync()
    {
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
        return Task.CompletedTask;
    }
    
    ~AuthTestingBase()
    {
        Context.Dispose();
    }
}