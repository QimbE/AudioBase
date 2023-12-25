using Application.GraphQL;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // GraphQL configuration
        services.AddGraphQLServer()
            .ConfigureHotChocolateTypes()
            .ConfigurePipeline();
        
        return services;
    }
}