using System.Security.Cryptography;
using Infrastructure.Data;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureTests;

public abstract class InfrastructureTestBase
{
    protected static InsertOutboxMessageInterceptor Interceptor = new();

    protected  readonly TestDbContext Context;
    
    public InfrastructureTestBase(Type toTakeName)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder
            .AddInterceptors(Interceptor)
            .UseInMemoryDatabase(toTakeName.Name);
        
        Context = new TestDbContext(builder.Options);
    }

    public void RecreateDatabase()
    {
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }
}