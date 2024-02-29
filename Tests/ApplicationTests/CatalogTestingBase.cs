using Infrastructure.Data;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace ApplicationTests;

public abstract class CatalogTestingBase<T> 
    where T : CatalogTestingBase<T>
{
    protected readonly TestDbContext Context;
    
    protected static InsertOutboxMessageInterceptor Interceptor = new();
    
    public CatalogTestingBase()
    {
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder
            .AddInterceptors(Interceptor)
            .UseInMemoryDatabase(typeof(T).Name);

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