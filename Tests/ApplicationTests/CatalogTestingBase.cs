using Infrastructure.Data;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTests;

public abstract class CatalogTestingBase
{
    protected readonly TestDbContext Context;
    
    protected static InsertOutboxMessageInterceptor Interceptor = new();
    
    public CatalogTestingBase(Type toTakeName)
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder
            .AddInterceptors(Interceptor)
            .UseInMemoryDatabase(toTakeName.Name);

        Context = new TestDbContext(builder.Options);
    }

    public void RecreateDbContext()
    {
        Context.Database.EnsureDeleted();
        Context.Database.EnsureCreated();
    }
    
    ~CatalogTestingBase()
    {
        Context.Dispose();
    }
}