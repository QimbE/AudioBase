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
}