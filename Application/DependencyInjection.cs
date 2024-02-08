using Application.Behaviors;
using Application.ExceptionHandlers;
using Application.GraphQL;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
            config.AddPipelineBehaviors();
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        
        // GraphQL configuration
        services.AddGraphQLServer()
            .ConfigureHotChocolateTypes()
            .UseAutomaticPersistedQueryPipeline()
            .AddRedisQueryStorage(
                s => s.GetRequiredService<IConnectionMultiplexer>().GetDatabase(),
                TimeSpan.FromMinutes(2)
                );
        
        return services;
    }
}