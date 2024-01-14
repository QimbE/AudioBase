using Application.Authentication;
using Infrastructure.Authentication;
using Infrastructure.Data;
using Infrastructure.Options;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace ApplicationTests.Authentication;

public abstract class AuthTestingBase
{
    protected readonly TestDbContext Context;
    protected readonly IJwtProvider JwtProvider;
    protected readonly IHashProvider HashProvider;
    
    protected static InsertOutboxMessageInterceptor Interceptor = new();
    
    public AuthTestingBase(Type toTakeName)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder
            .AddInterceptors(Interceptor)
            .UseInMemoryDatabase(toTakeName.Name);

        Context = new TestDbContext(builder.Options);
        
        var sectionMock = new JwtSettings
        {
            Issuer = "huh",
            Audience = "bimbimbambam",
            Key = "hehehehuhhehehehehehehehuhhwhwhwhhh",
            ExpiryTime = TimeSpan.FromMinutes(30)
        };
        
        var configMock = Substitute.For<IOptions<JwtSettings>>();

        configMock.Value.Returns(sectionMock);

        JwtProvider = new JwtProvider(configMock);
        HashProvider = new HashProvider();
    }

    public void RecreateDbContext()
    {
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }
    
    ~AuthTestingBase()
    {
        Context.Dispose();
    }
}