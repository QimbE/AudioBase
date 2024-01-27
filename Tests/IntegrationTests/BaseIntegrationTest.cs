using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

public abstract class BaseIntegrationTest
    : IClassFixture<IntegrationTestWebAppFactory>
{
    protected readonly IntegrationTestWebAppFactory Factory;
    
    public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        Factory = factory;
    }

    public Task RecreateDatabase()
    {
        using var scope = Factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        return Task.CompletedTask;
    }
}