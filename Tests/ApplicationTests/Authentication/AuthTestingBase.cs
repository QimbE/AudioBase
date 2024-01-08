using Application.Authentication;
using Infrastructure.Authentication;
using Infrastructure.Data;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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