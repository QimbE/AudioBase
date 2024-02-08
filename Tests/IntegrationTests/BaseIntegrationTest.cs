using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests;

[CollectionDefinition(nameof(IntegrationTestCollection), DisableParallelization = true)]
public class IntegrationTestCollection: ICollectionFixture<IntegrationTestWebAppFactory>
{
    
}

[Collection(nameof(IntegrationTestCollection))]
public abstract class BaseIntegrationTest: IAsyncLifetime
{
    protected readonly IntegrationTestWebAppFactory Factory;
    
    protected readonly HttpClient HttpClient;
    
    protected readonly IServiceScope Scope;
    
    public BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        Factory = factory;
        HttpClient = Factory.CreateClient();
        Scope = Factory.Services.CreateScope();
    }

    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        await Factory.ResetDatabaseAsync();
        Scope.Dispose();
    }
}